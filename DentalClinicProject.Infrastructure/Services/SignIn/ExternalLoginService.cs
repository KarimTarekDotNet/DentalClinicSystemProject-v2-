using DentalClinicProject.Core.DTOs.Auth;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Core.ViewModels;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DentalClinicProject.Infrastructure.Services.SignIn
{
    public class ExternalLoginService : IExternalLoginService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IRedisService _redisService;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ExternalLoginService> _logger;

        public ExternalLoginService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            IRedisService redisService,
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ExternalLoginService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _redisService = redisService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApiResponse<AuthResult>> ExternalSignInAsync(ExternalLoginCallbackDTO CallbackDTO)
        {
            try
            {
                _logger.LogInformation("External sign-in attempt with provider: {Provider}, email: {Email}",
                    CallbackDTO.Provider, CallbackDTO.Email);

                if (string.IsNullOrWhiteSpace(CallbackDTO.Provider) || string.IsNullOrWhiteSpace(CallbackDTO.ProviderKey))
                {
                    _logger.LogWarning("External sign-in failed: Provider or ProviderKey is empty");
                    return Fail(400, "Invalid external login data");
                }

                if (string.IsNullOrWhiteSpace(CallbackDTO.Email))
                {
                    _logger.LogWarning("External sign-in failed: Email is required");
                    return Fail(400, "Email is required from external provider");
                }

                // Parse provider enum
                if (!Enum.TryParse<Provider>(CallbackDTO.Provider, true, out var providerEnum))
                {
                    _logger.LogWarning("External sign-in failed: Invalid provider {Provider}", CallbackDTO.Provider);
                    return Fail(400, "Invalid provider");
                }

                // Check if user exists by email
                var user = await _userManager.FindByEmailAsync(CallbackDTO.Email);

                if (user == null)
                {
                    // Create new user
                    _logger.LogInformation("Creating new user from external provider: {Provider}", CallbackDTO.Provider);

                    user = new AppUser
                    {
                        UserName = CallbackDTO.Email.Split('@')[0] + "_" + Guid.NewGuid().ToString().Substring(0, 8),
                        Email = CallbackDTO.Email,
                        EmailConfirmed = true, // External providers verify email
                        FirstName = CallbackDTO.FirstName ?? "User",
                        LastName = CallbackDTO.LastName ?? "External",
                        Provider = providerEnum,
                        ProviderId = CallbackDTO.ProviderKey
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        _logger.LogError("User creation failed for external login: {Errors}",
                            string.Join(", ", createResult.Errors.Select(e => e.Description)));
                        return Fail(400, "Failed to create user", createResult.Errors.Select(e => e.Description));
                    }

                    // Add default role (user)
                    await _userManager.AddToRoleAsync(user, Role.User.ToString());

                    // Add external login
                    var addLoginResult = await _userManager.AddLoginAsync(user,
                        new UserLoginInfo(CallbackDTO.Provider, 
                        CallbackDTO.ProviderKey, CallbackDTO.Provider));

                    if (!addLoginResult.Succeeded)
                    {
                        _logger.LogError("Failed to add external login: {Errors}",
                            string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));
                    }

                    // Cache user in Redis
                    await _redisService.SetAsync(RedisKeys.UserByEmail(CallbackDTO.Email), user.Id, TimeSpan.FromHours(24));
                    await _redisService.SetAsync(RedisKeys.UserByUsername(user.UserName!), user.Id, TimeSpan.FromHours(24));
                    await _redisService.SetAsync(RedisKeys.UserById(user.Id), user.Id, TimeSpan.FromHours(24));

                    _logger.LogInformation("New user {UserId} created successfully from {Provider}", user.Id, CallbackDTO.Provider);
                }
                else
                {
                    // User exists - check if external login is already linked
                    var existingLogin = await _userManager.FindByLoginAsync(CallbackDTO.Provider,
                        CallbackDTO.ProviderKey);

                    if (existingLogin == null)
                    {
                        // Link external login to existing user
                        var addLoginResult = await _userManager.AddLoginAsync(user,
                            new UserLoginInfo(CallbackDTO.Provider,
                            CallbackDTO.ProviderKey, CallbackDTO.Provider));

                        if (!addLoginResult.Succeeded)
                        {
                            _logger.LogError("Failed to link external login to existing user: {Errors}",
                                string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));
                            return Fail(400, "Failed to link external login", addLoginResult.Errors.Select(e => e.Description));
                        }

                        _logger.LogInformation("External login {Provider} linked to existing user {UserId}",
                            CallbackDTO.Provider, user.Id);
                    }

                    // Update provider info if needed
                    if (user.Provider == Provider.Local)
                    {
                        user.Provider = providerEnum;
                        user.ProviderId = CallbackDTO.ProviderKey;
                        await _userManager.UpdateAsync(user);
                    }
                }

                // Generate tokens
                var accessToken = await _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();
                await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

                // Cache refresh token in Redis
                var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
                string refreshKey = RedisKeys.RefreshToken(user.Id, ipAddress);
                await _redisService.SetAsync(refreshKey, refreshToken, TimeSpan.FromDays(15));

                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("User {UserId} signed in successfully via {Provider}", user.Id, CallbackDTO.Provider);

                return new ApiResponse<AuthResult>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "External sign-in successful",
                    Data = new AuthResult
                    {
                        Succeeded = true,
                        Token = accessToken,
                        RefreshToken = refreshToken,
                        UserId = user.Id,
                        Email = user.Email,
                        Username = user.UserName,
                        Role = roles.FirstOrDefault() ?? "Patient"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during external sign-in with provider: {Provider}", CallbackDTO.Provider);
                return Fail(500, "An error occurred during external sign-in");
            }
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(Provider provider, string redirectUrl)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider.ToString(), redirectUrl);
            return properties;
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
}
