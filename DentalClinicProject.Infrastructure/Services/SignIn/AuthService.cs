using AutoMapper;
using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.DTOs.Auth;
using DentalClinicProject.Core.Entities.AuthModel;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Core.ViewModels;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class AuthService : IAuthService
{
    #region Objects
    private readonly IMailService _mailService;
    private readonly IPhoneService _phoneService;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IRedisService _redisService;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        IMapper mapper, ITokenService tokenService, ApplicationDbContext context,
        IRedisService redisService, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger,
        IMailService mailService, IPhoneService phoneService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _tokenService = tokenService;
        _context = context;
        _redisService = redisService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _mailService = mailService;
        _phoneService = phoneService;
    }
    #endregion

    public async Task<ApiResponse<AuthResult>> Register(RegisterDTO dto)
    {
        try
        {
            _logger.LogInformation("Registration attempt for email: {Email}, username: {Username}", dto.Email, dto.UserName);

            if (dto == null)
            {
                _logger.LogWarning("Registration failed: DTO is null");
                return Fail(400, "Invalid registration data");
            }

            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.UserName))
            {
                _logger.LogWarning("Registration failed: Email or username is empty");
                return Fail(400, "Email and username are required");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                _logger.LogWarning("Registration failed: Password is empty for user {Username}", dto.UserName);
                return Fail(400, "Password is required");
            }

            if (dto.Role == Role.Doctor || dto.Role == Role.Admin || dto.Role == Role.DelivaryMan)
                return Fail(400, "Only the admin can add you as an employee");

            var user = _mapper.Map<AppUser>(dto);

            var exists = await Helper.CheckExists(dto.Email, dto.UserName, _userManager);
            if (!exists)
            {
                _logger.LogWarning("Registration failed: Email {Email} or username {Username} already exists", dto.Email, dto.UserName);
                return Fail(400, "Email or username already exists");
            }

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("User creation failed for {Email}: {Errors}", dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return Fail(400, "User creation failed", result.Errors.Select(e => e.Description));
            }

            await _userManager.AddToRoleAsync(user, dto.Role.ToString());
            _logger.LogInformation("User {UserId} registered successfully with email {Email} and role {Role}", user.Id, dto.Email, dto.Role);

            await _redisService.SetAsync(RedisKeys.UserByEmail(dto.Email), user.Id, TimeSpan.FromHours(24));
            await _redisService.SetAsync(RedisKeys.UserByUsername(dto.UserName), user.Id, TimeSpan.FromHours(24));
            await _redisService.SetAsync(RedisKeys.UserById(user.Id), user.Id, TimeSpan.FromHours(24));

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                await _redisService.SetAsync(RedisKeys.UserByPhone(dto.PhoneNumber), user.Id, TimeSpan.FromHours(24));

            _logger.LogInformation("User {UserId} cached in Redis", user.Id);

            await Helper.SendVerificationEmailAsync(dto.Email, _redisService, _mailService, _logger);

            var sessionToken = Helper.GenerateSecureToken();
            await _redisService.SetAsync(RedisKeys.PendingVerificationSession(sessionToken), user.Id, TimeSpan.FromHours(1));
            await _redisService.SetAsync(RedisKeys.PendingVerificationByUserId(user.Id), sessionToken, TimeSpan.FromHours(1));

            return new ApiResponse<AuthResult>
            {
                Success = true,
                StatusCode = 200,
                Message = "Account created successfully. Please verify your email first.",
                Data = new AuthResult
                {
                    Succeeded = false,
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Token = sessionToken
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for email: {Email}", dto?.Email);
            return Fail(500, "An error occurred during registration");
        }
    }

    public async Task<ApiResponse<AuthResult>> Login(LoginDTO dto)
    {
        try
        {
            _logger.LogInformation("Login attempt with identifier");

            if (dto == null)
            {
                _logger.LogWarning("Login failed: DTO is null");
                return Fail(400, "Invalid login data");
            }

            if (string.IsNullOrWhiteSpace(dto.Identifier))
            {
                _logger.LogWarning("Login failed: Identifier is empty");
                return Fail(400, "Email, Username, or Phone Number is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                _logger.LogWarning("Login failed: Password is empty");
                return Fail(400, "Password is required");
            }

            var identifier = dto.Identifier.Trim();
            var (user, loginMethod) = await ResolveUserAsync(identifier);

            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found using {LoginMethod}", loginMethod);
                return Fail(400, "Invalid credentials");
            }

            var check = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
            if (check.IsLockedOut)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                var remainingTime = lockoutEnd - DateTimeOffset.UtcNow;
                _logger.LogWarning("Account is locked. Try again after {Minutes} minutes.", remainingTime?.Minutes);
                return Fail(400, $"Your account is locked. Try again after {remainingTime?.Minutes} minutes.");
            }

            if (!check.Succeeded)
            {
                _logger.LogWarning("Login failed: Invalid password for user {UserId}", user.Id);
                return Fail(400, "Invalid credentials");
            }

            _logger.LogInformation("Checking verification for user {UserId} - EmailConfirmed: {EmailConfirmed}, PhoneConfirmed: {PhoneConfirmed}",
                user.Id, user.EmailConfirmed, user.PhoneNumberConfirmed);

            bool isEmployee = await IsEmployeeAsync(user);

            if (loginMethod == "PhoneNumber")
            {
                if (!user.PhoneNumberConfirmed)
                {
                    _logger.LogWarning("Login failed: Phone not verified for user {UserId}", user.Id);
                    return Fail(400, "Phone not verified");
                }

                if (isEmployee)
                    return await BuildEmployeeLoginResponse(user);

                await Helper.SendVerificationPhoneAsync(user.PhoneNumber!, _redisService, _phoneService, _logger);
                _logger.LogInformation("2FA code sent to phone for user {UserId}", user.Id);
                return Build2FAResponse(user, "Verification code sent to your phone. Please verify to complete login.");
            }
            else // Email or Username
            {
                if (!user.EmailConfirmed)
                {
                    _logger.LogWarning("Login failed: Email not verified for user {UserId}", user.Id);
                    return Fail(400, "Email not verified");
                }

                if (isEmployee)
                    return await BuildEmployeeLoginResponse(user);

                await Helper.SendVerificationEmailAsync(user.Email!, _redisService, _mailService, _logger);
                _logger.LogInformation("2FA code sent to email for user {UserId}", user.Id);
                return Build2FAResponse(user, "Verification code sent to your email. Please verify to complete login.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login");
            return Fail(500, "An error occurred during login");
        }
    }

    public async Task<ApiResponse<bool>> LogoutAsync(string userId, string accessToken)
    {
        try
        {
            _logger.LogInformation("Logout attempt for user: {UserId}", userId);

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(accessToken))
                return new ApiResponse<bool> { Success = false, StatusCode = 400, Message = "UserId and access token are required" };

            var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);

            await _redisService.SetAsync(RedisKeys.BlacklistedAccessToken(userId, accessToken), "blacklisted", TimeSpan.FromMinutes(30));

            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && !t.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revoke(ipAddress, "Logout");
                await _redisService.DeleteAsync(RedisKeys.RefreshToken(userId, token.Id.ToString()));
            }

            _context.RefreshTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} logged out successfully", userId);

            return new ApiResponse<bool>
            {
                Success = true,
                StatusCode = 200,
                Message = "Logged out successfully",
                Data = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user {UserId}", userId);
            return new ApiResponse<bool> { Success = false, StatusCode = 500, Message = "An error occurred during logout" };
        }
    }

    public async Task<ApiResponse<AuthResult>> VerifyLoginCode(string identifier, string code)
    {
        try
        {
            _logger.LogInformation("Login code verification attempt for identifier: {Identifier}", identifier);

            if (string.IsNullOrWhiteSpace(identifier))
            {
                _logger.LogWarning("Verification failed: Identifier is empty");
                return Fail(400, "Identifier is required");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                _logger.LogWarning("Verification failed: Code is empty");
                return Fail(400, "Verification code is required");
            }

            var identifierTrimmed = identifier.Trim();
            AppUser? user = null;
            string verificationKey = string.Empty;

            if (identifierTrimmed.Contains("@"))
            {
                verificationKey = RedisKeys.EmailVerificationCode(identifierTrimmed, code);
                var savedCode = await _redisService.GetAsync(verificationKey);

                if (string.IsNullOrEmpty(savedCode))
                {
                    _logger.LogWarning("Verification failed: Invalid or expired code for email");
                    return Fail(400, "Invalid or expired verification code");
                }

                user = await _userManager.FindByEmailAsync(identifierTrimmed);
            }
            else if (identifierTrimmed.StartsWith("+") || identifierTrimmed.All(char.IsDigit))
            {
                verificationKey = RedisKeys.PhoneVerificationCode(identifierTrimmed);
                var savedCode = await _redisService.GetAsync(verificationKey);

                if (string.IsNullOrEmpty(savedCode))
                {
                    _logger.LogWarning("Verification failed: Invalid or expired code for phone");
                    return Fail(400, "Invalid or expired verification code");
                }

                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber != null && u.PhoneNumber == identifierTrimmed);
            }
            else
            {
                user = await _userManager.FindByNameAsync(identifierTrimmed);
                if (user != null)
                {
                    verificationKey = RedisKeys.EmailVerificationCode(user.Email!, code);
                    var savedCode = await _redisService.GetAsync(verificationKey);

                    if (string.IsNullOrEmpty(savedCode))
                    {
                        _logger.LogWarning("Verification failed: Invalid or expired code for username");
                        return Fail(400, "Invalid or expired verification code");
                    }
                }
            }

            if (user == null)
            {
                _logger.LogWarning("Verification failed: User not found");
                return Fail(400, "User not found");
            }

            await _redisService.DeleteAsync(verificationKey);

            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

            var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
            await _redisService.SetAsync(RedisKeys.RefreshToken(user.Id, ipAddress), refreshToken, TimeSpan.FromDays(15));

            var roles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation("User {UserId} logged in successfully after 2FA verification", user.Id);

            return new ApiResponse<AuthResult>
            {
                Success = true,
                StatusCode = 200,
                Message = "Login successful",
                Data = new AuthResult
                {
                    Succeeded = true,
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Role = roles.FirstOrDefault()
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login code verification");
            return Fail(500, "An error occurred during verification");
        }
    }

    public async Task<ApiResponse<AuthResult>> RefreshTokenAsync(RefreshTokenDTO dto)
    {
        try
        {
            var refreshToken = (await _context.RefreshTokens.ToListAsync())
                               .FirstOrDefault(r => r.Token == dto.RefreshToken && r.IsActive);

            if (refreshToken == null)
                return Fail(404, "Invalid or expired refresh token");

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);
            if (user == null)
                return Fail(404, "User not found");

            var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);

            refreshToken.Revoke(ipAddress, "Refresh");
            await _redisService.DeleteAsync(RedisKeys.RefreshToken(user.Id, refreshToken.Id.ToString()));
            await _context.SaveChangesAsync();

            var newAccessToken = await _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            var savedRefresh = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(15),
                CreatedByIp = ipAddress
            };

            await _context.RefreshTokens.AddAsync(savedRefresh);
            await _context.SaveChangesAsync();

            await _redisService.SetAsync(RedisKeys.RefreshToken(user.Id, savedRefresh.Id.ToString()), newRefreshToken, TimeSpan.FromDays(15));

            var roles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation("Token refreshed for user {UserId}", user.Id);

            return new ApiResponse<AuthResult>
            {
                Success = true,
                StatusCode = 200,
                Message = "Token refreshed successfully",
                Data = new AuthResult
                {
                    Succeeded = true,
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    UserId = user.Id,
                    Email = user.Email!,
                    Username = user.UserName!,
                    Role = roles.FirstOrDefault() ?? string.Empty
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return Fail(500, "An error occurred while refreshing token");
        }
    }

    #region Private Helpers

    private async Task<(AppUser? user, string loginMethod)> ResolveUserAsync(string identifier)
    {
        AppUser? user = null;
        string loginMethod;

        if (identifier.Contains("@"))
        {
            loginMethod = "Email";
            var cachedUserId = await _redisService.GetAsync(RedisKeys.UserByEmail(identifier));
            if (!string.IsNullOrEmpty(cachedUserId))
            {
                user = await _userManager.FindByIdAsync(cachedUserId);
                _logger.LogInformation("User found in Redis cache by Email");
            }
            else
            {
                user = await _userManager.FindByEmailAsync(identifier);
                if (user != null)
                {
                    await _redisService.SetAsync(RedisKeys.UserByEmail(identifier), user.Id, TimeSpan.FromHours(24));
                    _logger.LogInformation("User cached in Redis by Email");
                }
            }
        }
        else if (identifier.StartsWith("+") || identifier.All(char.IsDigit))
        {
            loginMethod = "PhoneNumber";
            user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber != null && u.PhoneNumber == identifier && u.PhoneNumberConfirmed);

            if (user != null)
            {
                await _redisService.SetAsync(RedisKeys.UserByPhone(identifier), user.Id, TimeSpan.FromHours(24));
                _logger.LogInformation("Verified user found and cached in Redis by Phone");
            }
            else
            {
                _logger.LogWarning("No verified user found with phone number");
            }
        }
        else
        {
            loginMethod = "Username";
            var cachedUserId = await _redisService.GetAsync(RedisKeys.UserByUsername(identifier));
            if (!string.IsNullOrEmpty(cachedUserId))
            {
                user = await _userManager.FindByIdAsync(cachedUserId);
                _logger.LogInformation("User found in Redis cache by Username");
            }
            else
            {
                user = await _userManager.FindByNameAsync(identifier);
                if (user != null)
                {
                    await _redisService.SetAsync(RedisKeys.UserByUsername(identifier), user.Id, TimeSpan.FromHours(24));
                    _logger.LogInformation("User cached in Redis by Username");
                }
            }
        }

        return (user, loginMethod);
    }

    private async Task<bool> IsEmployeeAsync(AppUser user)
    {
        return await _userManager.IsInRoleAsync(user, "Admin")
            || await _userManager.IsInRoleAsync(user, "Doctor")
            || await _userManager.IsInRoleAsync(user, "DelivaryMan");
    }

    private async Task<ApiResponse<AuthResult>> BuildEmployeeLoginResponse(AppUser user)
    {
        var accessToken = await _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);
        var roles = await _userManager.GetRolesAsync(user);

        _logger.LogInformation("Employee {UserId} logged in successfully", user.Id);

        return new ApiResponse<AuthResult>
        {
            Success = true,
            StatusCode = 200,
            Message = "Login successful",
            Data = new AuthResult
            {
                Succeeded = true,
                UserId = user.Id,
                Email = user.Email,
                Username = user.UserName,
                Role = roles.FirstOrDefault(),
                Token = accessToken,
                RefreshToken = refreshToken
            }
        };
    }

    private ApiResponse<AuthResult> Build2FAResponse(AppUser user, string message)
    {
        return new ApiResponse<AuthResult>
        {
            Success = true,
            StatusCode = 200,
            Message = message,
            Data = new AuthResult
            {
                Succeeded = false,
                UserId = user.Id,
                Email = user.Email,
                Username = user.UserName
            }
        };
    }

    private ApiResponse<AuthResult> Fail(int code, string message, IEnumerable<string>? errors = null)
    {
        return new ApiResponse<AuthResult>
        {
            Success = false,
            StatusCode = code,
            Message = message,
            Errors = errors ?? Enumerable.Empty<string>()
        };
    }

    #endregion
}