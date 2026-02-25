using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Core.ViewModels;
using DentalClinicProject.Infrastructure.Services.AuthHelper;
using DentalClinicProject.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DentalClinicProject.Infrastructure.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IRedisService _redisService;
        private readonly ITokenService _tokenService;
        private readonly IMailService mailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EmailVerificationService> _logger;
        private readonly ILogger<AuthService> _AuthLogger;

        public EmailVerificationService(
            UserManager<AppUser> userManager,
            IRedisService redisService,
            ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<EmailVerificationService> logger,
            IMailService mailService,
            ILogger<AuthService> authLogger)
        {
            _userManager = userManager;
            _redisService = redisService;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            this.mailService = mailService;
            _AuthLogger = authLogger;
        }

        public async Task<ApiResponse<AuthResult>> VerifyEmailAsync(string email, string code)
        {
            try
            {
                _logger.LogInformation("Email verification attempt for: {Email}", email);

                // Validation checks
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("Email verification failed: Email is empty");
                    return new ApiResponse<AuthResult>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Email is required"
                    };
                }

                if (string.IsNullOrWhiteSpace(code))
                {
                    _logger.LogWarning("Email verification failed: Code is empty for email {Email}", email);
                    return new ApiResponse<AuthResult>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Verification code is required"
                    };
                }

                string key = $"{email}:Code:{code}";
                var savedCode = await _redisService.GetAsync(key);

                if (savedCode == null)
                {
                    _logger.LogWarning("Email verification failed: Invalid or expired code for email {Email}", email);
                    return new ApiResponse<AuthResult>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Invalid or expired verification code"
                    };
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("Email verification failed: User not found for email {Email}", email);
                    return new ApiResponse<AuthResult>
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "User not found"
                    };
                }

                if (user.EmailConfirmed)
                {
                    _logger.LogInformation("Email already verified for user {UserId}", user.Id);
                    return new ApiResponse<AuthResult>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Email is already verified"
                    };
                }

                user.EmailConfirmed = true;
                var updateResult = await _userManager.UpdateAsync(user);
                
                if (!updateResult.Succeeded)
                {
                    _logger.LogError("Failed to update user {UserId} email confirmation status: {Errors}", 
                        user.Id, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                    return new ApiResponse<AuthResult>
                    {
                        Success = false,
                        StatusCode = 500,
                        Message = "Failed to verify email"
                    };
                }

                await _redisService.DeleteAsync(key);

                var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
                await _tokenService.RevokeAllUserTokensAsync(user.Id, ipAddress, "EmailVerification");

                var accessToken = await _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();
                var refresh = await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

                string refreshKey = $"{user.Id}:User:{refresh.CreatedByIp}";
                await _redisService.SetAsync(refreshKey, refreshToken, TimeSpan.FromDays(15));

                _logger.LogInformation("Email verified successfully for user {UserId}", user.Id);

                return new ApiResponse<AuthResult>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Email verified successfully",
                    Data = new AuthResult
                    {
                        Succeeded = true,
                        Token = accessToken,
                        RefreshToken = refreshToken,
                        UserId = user.Id,
                        Email = user.Email,
                        Username = user.UserName
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during email verification for: {Email}", email);
                return new ApiResponse<AuthResult>
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "An error occurred during email verification"
                };
            }
        }

        public async Task<bool> ResendEmailVerificationCodeAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Resending email verification code for user: {UserId}", userId);

                // Validation checks
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("Resend email verification failed: UserId is empty");
                    return false;
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Resend email verification failed: User not found for userId {UserId}", userId);
                    return false;
                }

                if (user.EmailConfirmed)
                {
                    _logger.LogInformation("Resend email verification skipped: email already verified for user {UserId}", userId);
                    return false;
                }

                var rateLimitKey = $"User:{user.Email}";
                var last = await _redisService.GetAsync(rateLimitKey);

                if (!string.IsNullOrEmpty(last))
                {
                    _logger.LogWarning("Resend email verification failed: Rate limit exceeded for user {UserId}", userId);
                    return false;
                }

                await Helper.SendVerificationEmailAsync(user.Email!, _redisService, mailService, _AuthLogger);
                await _redisService.SetAsync(rateLimitKey, "sent", TimeSpan.FromMinutes(1));

                _logger.LogInformation("email verification code resent successfully to user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending email verification code for user {UserId}", userId);
                return false;
            }
        }
    }
}