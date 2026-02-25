using DentalClinicProject.Core.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace DentalClinicProject.Infrastructure.Services.AuthHelper
{
    public class PhoneService : IPhoneService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PhoneService> _logger;

        public PhoneService(IConfiguration configuration, ILogger<PhoneService> logger)
        {
            this._configuration = configuration;
            _logger = logger;
            _logger.LogInformation("SID: {Sid}", _configuration["Twilio:AccountSID"]);
            TwilioClient.Init(_configuration["Twilio:AccountSID"], _configuration["Twilio:AuthToken"]);
        }

        public async Task SendCodeAsync(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                {
                    _logger.LogWarning("Attempted to send verification code to empty phone number");
                    throw new ArgumentException("Phone number cannot be empty");
                }

                if (!phone.StartsWith("+"))
                {
                    _logger.LogWarning("Phone number {Phone} is not in international format", phone);
                    throw new ArgumentException("Phone number must be in international format (e.g., +201234567890)");
                }

                await VerificationResource.CreateAsync(to: phone, channel: "sms", pathServiceSid: _configuration["Twilio:VerifyServiceSID"]);

                _logger.LogInformation("Verification code sent successfully to {Phone}", phone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification code to {Phone}", phone);
                throw new InvalidOperationException("Failed to send verification code. Please try again later.", ex);
            }
        }

        public async Task<bool> VerifyCodeAsync(string phone, string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(code))
                {
                    _logger.LogWarning("Attempted to verify with empty phone or code");
                    return false;
                }
                var result = await VerificationCheckResource
                    .CreateAsync(to: phone, code: code, pathServiceSid: _configuration["Twilio:VerifyServiceSID"]);

                if (result.Status == "approved")
                {
                    _logger.LogInformation("Phone {Phone} verified successfully", phone);
                    return true;
                }

                if (result.Status == "pending")
                {
                    _logger.LogWarning("Verification code for {Phone} is still pending", phone);
                    return false;
                }

                _logger.LogWarning("Unexpected verification status {Status} for {Phone}", result.Status, phone);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify code for {Phone}", phone);
                return false;
            }
        }
    }
}