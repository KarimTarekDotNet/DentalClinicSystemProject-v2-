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
        private readonly IPhoneService _phoneService;
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
            IPhoneService phoneService,
            ILogger<AuthService> authLogger)
        {
            _userManager = userManager;
            _redisService = redisService;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            this.mailService = mailService;
            _phoneService = phoneService;
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

                // Normalize input
                email = email.Trim().ToLowerInvariant();
                code = code.Trim().ToUpperInvariant();
                
                string key = RedisKeys.EmailVerificationCode(email, code);
                _logger.LogInformation("Looking up verification code with key: {Key}", key);
                
                var savedCode = await _redisService.GetAsync(key);
                
                _logger.LogInformation("Redis lookup result - Key: {Key}, Found: {Found}, Value: {Value}", 
                    key, savedCode != null, savedCode ?? "null");

                if (savedCode == null)
                {
                    _logger.LogWarning("Email verification failed: Invalid or expired code for email {Email}, key: {Key}", email, key);
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

                // Delete active email verification code reference
                var activeCodeKey = RedisKeys.ActiveEmailVerificationCode(user.Email!);
                await _redisService.DeleteAsync(activeCodeKey);

                // Delete pending verification session token
                var userSessionKey = RedisKeys.PendingVerificationByUserId(user.Id);
                var sessionToken = await _redisService.GetAsync(userSessionKey);
                if (!string.IsNullOrEmpty(sessionToken))
                {
                    var sessionKey = RedisKeys.PendingVerificationSession(sessionToken);
                    await _redisService.DeleteAsync(sessionKey);
                    await _redisService.DeleteAsync(userSessionKey);
                    _logger.LogInformation("Deleted pending verification session for user {UserId}", user.Id);
                }

                // Try to send phone verification code if user has a phone number (non-blocking)
                if (!string.IsNullOrEmpty(user.PhoneNumber))
                {
                    try
                    {
                        _logger.LogInformation("Attempting to send phone verification code to user {UserId} after email confirmation", user.Id);
                        await Helper.SendVerificationPhoneAsync(user.PhoneNumber, _redisService, _phoneService, _AuthLogger);
                        _logger.LogInformation("Phone verification code sent successfully to user {UserId}", user.Id);
                    }
                    catch (Exception phoneEx)
                    {
                        // Log error but don't fail email verification
                        _logger.LogWarning(phoneEx, "Failed to send phone verification code to user {UserId}, but email verification succeeded", user.Id);
                    }
                }

                var ipAddress = IpAddressHelper.GetClientIpAddress(_httpContextAccessor);
                await _tokenService.RevokeAllUserTokensAsync(user.Id, ipAddress, "EmailVerification");

                var accessToken = await _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();
                var refresh = await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

                string refreshKey = RedisKeys.RefreshToken(user.Id, refresh.CreatedByIp!);
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

        public async Task<bool> ResendEmailVerificationCodeAsync(string sessionToken)
        {
            try
            {
                _logger.LogInformation("Resending email verification code with session token");

                // Validation check
                if (string.IsNullOrWhiteSpace(sessionToken))
                {
                    _logger.LogWarning("Resend email verification failed: Session token is empty");
                    return false;
                }

                // Get userId from session token in Redis
                var sessionKey = RedisKeys.PendingVerificationSession(sessionToken);
                var userId = await _redisService.GetAsync(sessionKey);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Resend email verification failed: Invalid or expired session token");
                    return false;
                }

                // Find user by ID
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Resend email verification failed: User not found for userId {UserId}", userId);
                    return false;
                }

                if (user.EmailConfirmed)
                {
                    _logger.LogInformation("Resend email verification skipped: email already verified for user {UserId}", user.Id);
                    // Delete session token since email is already verified
                    await _redisService.DeleteAsync(sessionKey);
                    return false;
                }

                // Rate limit: 1 request per minute per email
                var rateLimitKey = RedisKeys.RateLimitEmail(user.Email!);
                var last = await _redisService.GetAsync(rateLimitKey);

                if (!string.IsNullOrEmpty(last))
                {
                    _logger.LogWarning("Resend email verification failed: Rate limit exceeded for email {Email}", user.Email);
                    return false;
                }

                await Helper.SendVerificationEmailAsync(user.Email!, _redisService, mailService, _AuthLogger);
                await _redisService.SetAsync(rateLimitKey, "sent", TimeSpan.FromMinutes(1));

                _logger.LogInformation("Email verification code resent successfully to user {UserId}", user.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending email verification code");
                return false;
            }
        }
    }
}