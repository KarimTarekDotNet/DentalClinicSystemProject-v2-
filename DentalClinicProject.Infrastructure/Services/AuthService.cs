using AutoMapper;
using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.Entities.Users;
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

    public AuthService(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,
        IMapper mapper,ITokenService tokenService, ApplicationDbContext context,
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

    public async Task<ApiResponse<AuthResult>> Register(RegisterDTO dto)
    {
        try
        {
            _logger.LogInformation("Registration attempt for email: {Email}, username: {Username}", dto.Email, dto.UserName);

            // Validation checks
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

            var user = _mapper.Map<AppUser>(dto);

            var exists = await Helper.CheckExists(dto.Email, dto.UserName, _userManager);
            if (!exists)
            {
                _logger.LogWarning("Registration failed: Email {Email} or username {Username} already exists", dto.Email, dto.UserName);
                return Fail(400, "Email or username already exists");
            }

            // Create user in database
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("User creation failed for {Email}: {Errors}", dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return Fail(400, "User creation failed", result.Errors.Select(e => e.Description));
            }

            await _userManager.AddToRoleAsync(user, dto.Role.ToString());
            _logger.LogInformation("User {UserId} registered successfully with email {Email} and role {Role}", user.Id, dto.Email, dto.Role);

            // Cache user in Redis by email, username, and phone (if provided)
            await _redisService.SetAsync(RedisKeys.UserByEmail(dto.Email), user.Id, TimeSpan.FromHours(24));
            await _redisService.SetAsync(RedisKeys.UserByUsername(dto.UserName), user.Id, TimeSpan.FromHours(24));
            await _redisService.SetAsync(RedisKeys.UserById(user.Id), user.Id, TimeSpan.FromHours(24));
            
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                await _redisService.SetAsync(RedisKeys.UserByPhone(dto.PhoneNumber), user.Id, TimeSpan.FromHours(24));
            }
            
            _logger.LogInformation("User {UserId} cached in Redis", user.Id);

            // Always send email verification code
            await Helper.SendVerificationEmailAsync(dto.Email, _redisService, _mailService, _logger);
            
            // Generate temporary session token for resend operations (valid for 1 hour)
            var sessionToken = Helper.GenerateSecureToken();
            var sessionKey = RedisKeys.PendingVerificationSession(sessionToken);
            await _redisService.SetAsync(sessionKey, user.Id, TimeSpan.FromHours(1));
            
            // Store session token by userId for cleanup after verification
            var userSessionKey = RedisKeys.PendingVerificationByUserId(user.Id);
            await _redisService.SetAsync(userSessionKey, sessionToken, TimeSpan.FromHours(1));
            
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
                    Token = sessionToken // Temporary session token for resend
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

            // Validation checks
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

            // Determine identifier type and find user
            AppUser? user = null;
            string loginMethod = string.Empty;
            var identifier = dto.Identifier.Trim();
            string redisKey = string.Empty;

            // Check if identifier is an email
            if (identifier.Contains("@"))
            {
                loginMethod = "Email";
                redisKey = RedisKeys.UserByEmail(identifier);
                
                // Try Redis first
                var cachedUserId = await _redisService.GetAsync(redisKey);
                if (!string.IsNullOrEmpty(cachedUserId))
                {
                    user = await _userManager.FindByIdAsync(cachedUserId);
                    _logger.LogInformation("User found in Redis cache by Email");
                }
                else
                {
                    // Get from database and cache
                    user = await _userManager.FindByEmailAsync(identifier);
                    if (user != null)
                    {
                        await _redisService.SetAsync(redisKey, user.Id, TimeSpan.FromHours(24));
                        _logger.LogInformation("User cached in Redis by Email");
                    }
                }
            }
            // Check if identifier is a phone number (starts with + or contains only digits)
            else if (identifier.StartsWith("+") || identifier.All(char.IsDigit))
            {
                loginMethod = "PhoneNumber";
                redisKey = RedisKeys.UserByPhone(identifier);
                
                // Always get from database for phone to ensure we get the verified user
                // Don't use Redis cache for phone login to avoid getting wrong user
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber != null && 
                                              u.PhoneNumber == identifier && 
                                              u.PhoneNumberConfirmed);
                
                if (user != null)
                {
                    // Cache the verified user
                    await _redisService.SetAsync(redisKey, user.Id, TimeSpan.FromHours(24));
                    _logger.LogInformation("Verified user found and cached in Redis by Phone");
                }
                else
                {
                    _logger.LogWarning("No verified user found with phone number");
                }
            }
            // Otherwise, treat as username
            else
            {
                loginMethod = "Username";
                redisKey = RedisKeys.UserByUsername(identifier);
                
                // Try Redis first
                var cachedUserId = await _redisService.GetAsync(redisKey);
                if (!string.IsNullOrEmpty(cachedUserId))
                {
                    user = await _userManager.FindByIdAsync(cachedUserId);
                    _logger.LogInformation("User found in Redis cache by Username");
                }
                else
                {
                    // Get from database and cache
                    user = await _userManager.FindByNameAsync(identifier);
                    if (user != null)
                    {
                        await _redisService.SetAsync(redisKey, user.Id, TimeSpan.FromHours(24));
                        _logger.LogInformation("User cached in Redis by Username");
                    }
                }
            }

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

            // Check verification based on login method
            _logger.LogInformation("Checking verification for user {UserId} with login method {LoginMethod}", user.Id, loginMethod);
            _logger.LogInformation("User verification status - EmailConfirmed: {EmailConfirmed}, PhoneNumberConfirmed: {PhoneConfirmed}", 
                user.EmailConfirmed, user.PhoneNumberConfirmed);
            
            if (loginMethod == "Email")
            {
                if (!user.EmailConfirmed)
                {
                    _logger.LogWarning("Login failed: Email not verified for user {UserId}", user.Id);
                    return Fail(400, "Email not verified");
                }
                
                // Send verification code to email for 2FA
                await Helper.SendVerificationEmailAsync(user.Email!, _redisService, _mailService, _logger);
                _logger.LogInformation("2FA code sent to email for user {UserId}", user.Id);
                
                return new ApiResponse<AuthResult>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Verification code sent to your email. Please verify to complete login.",
                    Data = new AuthResult
                    {
                        Succeeded = false,
                        UserId = user.Id,
                        Email = user.Email,
                        Username = user.UserName
                    }
                };
            }
            else if (loginMethod == "PhoneNumber")
            {
                // Send verification code to phone for 2FA (no PhoneNumberConfirmed check)
                await Helper.SendVerificationPhoneAsync(user.PhoneNumber!, _redisService, _phoneService, _logger);
                _logger.LogInformation("2FA code sent to phone for user {UserId}", user.Id);
                
                return new ApiResponse<AuthResult>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Verification code sent to your phone. Please verify to complete login.",
                    Data = new AuthResult
                    {
                        Succeeded = false,
                        UserId = user.Id,
                        Email = user.Email,
                        Username = user.UserName,
                    }
                };
            }
            else // Username login
            {
                // For username login, require email verification
                if (!user.EmailConfirmed)
                {
                    _logger.LogWarning("Login failed: Email not verified for user {UserId}", user.Id);
                    return Fail(400, "Email not verified");
                }
                
                // Send verification code to email for 2FA
                await Helper.SendVerificationEmailAsync(user.Email!, _redisService, _mailService, _logger);
                _logger.LogInformation("2FA code sent to email for user {UserId}", user.Id);
                
                return new ApiResponse<AuthResult>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Verification code sent to your email. Please verify to complete login.",
                    Data = new AuthResult
                    {
                        Succeeded = false,
                        UserId = user.Id,
                        Email = user.Email,
                        Username = user.UserName
                    }
                };
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

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "UserId is required"
                };
            }

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Access token is required"
                };
            }

            var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
            
            // Try to get user from Redis first
            string userCacheKey = RedisKeys.UserById(userId);
            var cachedUserId = await _redisService.GetAsync(userCacheKey);
            
            AppUser? user = null;
            if (!string.IsNullOrEmpty(cachedUserId))
            {
                _logger.LogInformation("User found in Redis cache for logout");
                user = await _userManager.FindByIdAsync(userId);
            }
            else
            {
                _logger.LogInformation("User not in cache, fetching from database");
                user = await _userManager.FindByIdAsync(userId);
                
                if (user != null)
                {
                    // Cache user for future operations (24 hours)
                    await _redisService.SetAsync(userCacheKey, user.Id, TimeSpan.FromHours(24));
                }
            }

            if (user == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "User not found"
                };
            }

            // Blacklist access token in Redis for 30 minutes
            var blacklistKey = RedisKeys.BlacklistedAccessToken(userId, accessToken);
            await _redisService.SetAsync(blacklistKey, "blacklisted", TimeSpan.FromMinutes(30));

            // Try to get and delete refresh token from Redis
            string refreshKey = RedisKeys.RefreshToken(userId, ipAddress);
            var cachedRefreshToken = await _redisService.GetAsync(refreshKey);
            
            if (!string.IsNullOrEmpty(cachedRefreshToken))
            {
                _logger.LogInformation("Refresh token found in Redis, deleting from cache");
                await _redisService.DeleteAsync(refreshKey);
            }

            // Revoke refresh token from database
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId && !x.IsRevoked);
            if (token != null)
            {
                await _tokenService.RevokeUserTokensAsync(token.Id, userId, ipAddress, "Logout");
            }

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

            return new ApiResponse<bool>
            {
                Success = false,
                StatusCode = 500,
                Message = "An error occurred during logout"
            };
        }
    }

    public async Task<ApiResponse<bool>> LogoutAllAsync(string userId, string accessToken)
    {
        try
        {
            _logger.LogInformation("Logout all sessions attempt for user: {UserId}", userId);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "UserId is required"
                };
            }

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Access token is required"
                };
            }

            var key = $"{userId}:{accessToken}";
            await _redisService.SetAsync(key, "blacklisted", TimeSpan.FromMinutes(30));

            var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
            await _tokenService.RevokeAllUserTokensAsync(userId, ipAddress, "LogoutAll");

            return new ApiResponse<bool>
            {
                Success = true,
                StatusCode = 200,
                Message = "Logged out from all sessions successfully",
                Data = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout all for user {UserId}", userId);

            return new ApiResponse<bool>
            {
                Success = false,
                StatusCode = 500,
                Message = "An error occurred during logout all"
            };
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

            // Determine if identifier is email or phone
            if (identifierTrimmed.Contains("@"))
            {
                // Email verification
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
                // Phone verification
                verificationKey = RedisKeys.PhoneVerificationCode(identifierTrimmed);
                var savedCode = await _redisService.GetAsync(verificationKey);

                if (string.IsNullOrEmpty(savedCode))
                {
                    _logger.LogWarning("Verification failed: Invalid or expired code for phone");
                    return Fail(400, "Invalid or expired verification code");
                }

                // Find user by phone - should be unique after verification
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber != null && u.PhoneNumber == identifierTrimmed);
            }
            else
            {
                // Username - use email for verification
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

            // Delete the verification code from Redis
            await _redisService.DeleteAsync(verificationKey);

            // Generate tokens
            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenRecord = await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

            // Cache refresh token in Redis
            var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
            string refreshKey = RedisKeys.RefreshToken(user.Id, ipAddress);
            await _redisService.SetAsync(refreshKey, refreshToken, TimeSpan.FromDays(15));

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
}