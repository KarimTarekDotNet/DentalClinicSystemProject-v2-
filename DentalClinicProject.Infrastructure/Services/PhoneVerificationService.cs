using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DentalClinicProject.Infrastructure.Services
{
    public class PhoneVerificationService : IPhoneVerificationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhoneService _phoneService;
        private readonly IRedisService _redisService;
        private readonly ILogger<PhoneVerificationService> _logger;

        public PhoneVerificationService(
            UserManager<AppUser> userManager,
            IPhoneService phoneService,
            IRedisService redisService,
            ILogger<PhoneVerificationService> logger)
        {
            _userManager = userManager;
            _phoneService = phoneService;
            _redisService = redisService;
            _logger = logger;
        }

        public async Task<bool> SendPhoneVerificationCodeAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Sending phone verification code for user: {UserId}", userId);

                // Validation checks
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("Send phone verification failed: UserId is empty");
                    return false;
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Send phone verification failed: User not found for userId {UserId}", userId);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    _logger.LogWarning("Send phone verification failed: Phone number is empty for user {UserId}", userId);
                    return false;
                }

                if (user.PhoneNumberConfirmed)
                {
                    _logger.LogInformation("Phone already verified for user {UserId}", userId);
                    return false;
                }

                await _phoneService.SendCodeAsync(user.PhoneNumber);
                _logger.LogInformation("Phone verification code sent successfully to user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending phone verification code for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ResendPhoneVerificationCodeAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Resending phone verification code for user: {UserId}", userId);

                // Validation checks
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("Resend phone verification failed: UserId is empty");
                    return false;
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Resend phone verification failed: User not found for userId {UserId}", userId);
                    return false;
                }

                if (user.PhoneNumberConfirmed)
                {
                    _logger.LogInformation("Resend phone verification skipped: Phone already verified for user {UserId}", userId);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    _logger.LogWarning("Resend phone verification failed: Phone number is empty for user {UserId}", userId);
                    return false;
                }

                var rateLimitKey = $"User:{user.PhoneNumber}";
                var last = await _redisService.GetAsync(rateLimitKey);

                if (!string.IsNullOrEmpty(last))
                {
                    _logger.LogWarning("Resend phone verification failed: Rate limit exceeded for user {UserId}", userId);
                    return false;
                }

                await _phoneService.SendCodeAsync(user.PhoneNumber!);
                await _redisService.SetAsync(rateLimitKey, "sent", TimeSpan.FromMinutes(1));

                _logger.LogInformation("Phone verification code resent successfully to user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending phone verification code for user {UserId}", userId);
                return false;
            }
        }

        public async Task<ApiResponse<AuthResult>> VerifyPhoneAsync(string userId, string code)
        {
            try
            {
                _logger.LogInformation("Phone verification attempt for user: {UserId}", userId);

                // Validation checks
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("Phone verification failed: UserId is empty");
                    return Fail("UserId is required");
                }

                if (string.IsNullOrWhiteSpace(code))
                {
                    _logger.LogWarning("Phone verification failed: Code is empty for user {UserId}", userId);
                    return Fail("Verification code is required");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Phone verification failed: User not found for userId {UserId}", userId);
                    return Fail("User not found");
                }

                if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    _logger.LogWarning("Phone verification failed: Phone number is empty for user {UserId}", userId);
                    return Fail("Phone number not found");
                }

                if (user.PhoneNumberConfirmed)
                {
                    _logger.LogInformation("Phone already verified for user {UserId}", userId);
                    return Fail("Phone number is already verified");
                }

                var valid = await _phoneService.VerifyCodeAsync(user.PhoneNumber!, code);
                if (!valid)
                {
                    _logger.LogWarning("Phone verification failed: Invalid code for user {UserId}", userId);
                    return Fail("Invalid or expired verification code");
                }

                user.PhoneNumberConfirmed = true;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    _logger.LogError("Failed to update user {UserId} phone confirmation status: {Errors}", 
                        userId, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                    return Fail("Failed to verify phone number");
                }

                _logger.LogInformation("Phone verified successfully for user {UserId}", userId);

                return new ApiResponse<AuthResult>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Phone verified successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during phone verification for user {UserId}", userId);
                return Fail("An error occurred during phone verification");
            }
        }

        private ApiResponse<AuthResult> Fail(string message)
        {
            return new ApiResponse<AuthResult>
            {
                Success = false,
                StatusCode = 400,
                Message = message
            };
        }
    }
}