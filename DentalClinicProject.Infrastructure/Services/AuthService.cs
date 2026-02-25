using AutoMapper;
using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Core.ViewModels;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("User creation failed for {Email}: {Errors}", dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return Fail(400, "User creation failed", result.Errors.Select(e => e.Description));
            }

            await _userManager.AddToRoleAsync(user, dto.Role.ToString());
            _logger.LogInformation("User {UserId} registered successfully with email {Email} and role {Role}", user.Id, dto.Email, dto.Role);

            await Helper.SendVerificationEmailAsync(dto.Email, _redisService, _mailService, _logger);
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                await Helper.SendVerificationPhoneAsync(dto.PhoneNumber, _redisService, _phoneService, _logger);
            return new ApiResponse<AuthResult>
            {
                Success = true,
                StatusCode = 200,
                Message = "Account created successfully"
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
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

            // Validation checks
            if (dto == null)
            {
                _logger.LogWarning("Login failed: DTO is null");
                return Fail(400, "Invalid login data");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                _logger.LogWarning("Login failed: Email is empty");
                return Fail(400, "Email is required");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                _logger.LogWarning("Login failed: Password is empty for email {Email}", dto.Email);
                return Fail(400, "Password is required");
            }

            var user = await _userManager.FindByEmailAsync(dto.Email!);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email {Email}", dto.Email);
                return Fail(400, "Invalid credentials");
            }

            var check = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!check.Succeeded)
            {
                _logger.LogWarning("Login failed: Invalid password for user {UserId}", user.Id);
                return Fail(400, "Invalid credentials");
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Login failed: Email not verified for user {UserId}", user.Id);
                return Fail(400, "Email not verified");
            }

            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

            var roles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return new ApiResponse<AuthResult>
            {
                Success = true,
                StatusCode = 200,
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
            _logger.LogError(ex, "Unexpected error during login for email: {Email}", dto?.Email);
            return Fail(500, "An error occurred during login");
        }
    }

    public async Task LogoutAsync(string userId, string accessToken)
    {
        try
        {
            _logger.LogInformation("Logout attempt for user: {UserId}", userId);

            // Validation checks
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Logout failed: UserId is empty");
                throw new ArgumentException("UserId is required", nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogWarning("Logout failed: AccessToken is empty for user {UserId}", userId);
                throw new ArgumentException("AccessToken is required", nameof(accessToken));
            }

            // Blacklist the current access token for its remaining lifetime (30 minutes to match JWT expiry)
            var key = $"{userId}:{accessToken}";
            var blacklisted = await _redisService.SetAsync(key, "blacklisted", TimeSpan.FromMinutes(30));
            
            if (!blacklisted)
            {
                _logger.LogWarning("Failed to blacklist token for user {UserId}", userId);
            }

            // Revoke all refresh tokens (since we don't know which one is current)
            var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
            await _tokenService.RevokeAllUserTokensAsync(userId, ipAddress, "Logout");

            _logger.LogInformation("User {UserId} logged out successfully", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user {UserId}", userId);
            throw;
        }
    }

    public async Task LogoutAllAsync(string userId, string accessToken)
    {
        try
        {
            _logger.LogInformation("Logout all sessions attempt for user: {UserId}", userId);

            // Validation checks
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Logout all failed: UserId is empty");
                throw new ArgumentException("UserId is required", nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogWarning("Logout all failed: AccessToken is empty for user {UserId}", userId);
                throw new ArgumentException("AccessToken is required", nameof(accessToken));
            }

            var key = $"{userId}:{accessToken}";
            var blacklisted = await _redisService.SetAsync(key, "blacklisted", TimeSpan.FromMinutes(30));
            
            if (!blacklisted)
            {
                _logger.LogWarning("Failed to blacklist token for user {UserId} during logout all", userId);
            }

            // Revoke all refresh tokens
            var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
            await _tokenService.RevokeAllUserTokensAsync(userId, ipAddress, "LogoutAll");

            _logger.LogInformation("User {UserId} logged out from all sessions successfully", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout all for user {UserId}", userId);
            throw;
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