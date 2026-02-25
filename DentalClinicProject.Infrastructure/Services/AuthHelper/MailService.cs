using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace DentalClinicProject.Infrastructure.Services.AuthHelper
{
    public class MailService : Core.Interfaces.IServices.IMailService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<MailService> logger;

        public MailService(IConfiguration configuration, ILogger<MailService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task SendConfirmEmail(string to, string subject, string message, IList<IFormFile>? Attachments = null)
        {
            try
            {
                logger.LogInformation("Sending confirmation email to {Email} with subject: {Subject}", to, subject);

                // Validation checks
                if (string.IsNullOrWhiteSpace(to))
                {
                    logger.LogWarning("SendConfirmEmail failed: Recipient email is empty");
                    throw new ArgumentException("Recipient email is required", nameof(to));
                }

                if (string.IsNullOrWhiteSpace(subject))
                {
                    logger.LogWarning("SendConfirmEmail failed: Subject is empty for email {Email}", to);
                    throw new ArgumentException("Email subject is required", nameof(subject));
                }

                if (string.IsNullOrWhiteSpace(message))
                {
                    logger.LogWarning("SendConfirmEmail failed: Message body is empty for email {Email}", to);
                    throw new ArgumentException("Email message is required", nameof(message));
                }

                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(configuration["Mail:From"]!),
                    Subject = subject,
                };
                
                email.To.Add(MailboxAddress.Parse(to));

                var builder = new BodyBuilder();

                if(Attachments != null && Attachments.Any())
                {
                    logger.LogDebug("Processing {Count} attachments for email to {Email}", Attachments.Count, to);
                    byte[] fileBytes;
                    foreach(var attachment in Attachments)
                    {
                        if(attachment.Length > 0)
                        { 
                            using (var stream = new MemoryStream())
                            {
                                attachment.CopyTo(stream);
                                fileBytes = stream.ToArray();
                                await builder.Attachments.AddAsync(attachment.FileName, ContentType.Parse(attachment.ContentType));
                            }
                        }
                    }
                }

                builder.HtmlBody = message;
                email.Body = builder.ToMessageBody();
                email.From.Add(new MailboxAddress(configuration["Mail:Username"]!, configuration["Mail:From"]!));

                // connect SMTP 
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(configuration["Mail:Host"]!, int.Parse(configuration["Mail:Port"]!),
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync("apikey", configuration["Mail:Key"]!);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                logger.LogInformation("Email sent successfully to {Email}", to);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send email to {Email}", to);
                throw;
            }
        }

        public async Task SendVerificationCodeEmail(string email, string code)
        {
            try
            {
                logger.LogInformation("Sending verification code email to {Email}", email);

                // Validation checks
                if (string.IsNullOrWhiteSpace(email))
                {
                    logger.LogWarning("SendVerificationCodeEmail failed: Email is empty");
                    throw new ArgumentException("Email is required", nameof(email));
                }

                if (string.IsNullOrWhiteSpace(code))
                {
                    logger.LogWarning("SendVerificationCodeEmail failed: Code is empty for email {Email}", email);
                    throw new ArgumentException("Verification code is required", nameof(code));
                }

                var subject = "Email Verification Code";

                var body = $@"
                <div style='font-family: Arial, sans-serif; line-height:1.6; color:#333'>
                    <h2 style='color:#2c3e50;'>Verify Your Email Address</h2>
    
                    <p>Thank you for registering with us.</p>
    
                    <p>
                        To complete your registration, please use the verification code below:
                    </p>
    
                    <div style='margin:20px 0; padding:15px; background:#f4f6f8; 
                                text-align:center; font-size:22px; font-weight:bold; 
                                letter-spacing:3px; border-radius:6px;'>
                        {code}
                    </div>
    
                    <p>
                        This code will expire in <strong>15 minutes</strong>. 
                        If you did not request this, please ignore this email.
                    </p>
    
                    <hr style='margin-top:30px;'/>
    
                    <p style='font-size:12px; color:#888;'>
                        This is an automated message, please do not reply.
                    </p>
                </div>";

                await SendConfirmationEmail(email, subject, body);
                logger.LogInformation("Verification code email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send verification code email to {Email}", email);
                throw;
            }
        }

        public async Task SendConfirmationEmail(string email, string subject, string body)
        {
            try
            {
                logger.LogInformation("Sending confirmation email to {Email}", email);

                // Validation checks
                if (string.IsNullOrWhiteSpace(email))
                {
                    logger.LogWarning("SendConfirmationEmail failed: Email is empty");
                    throw new ArgumentException("Email is required", nameof(email));
                }

                await SendConfirmEmail(email, subject, body);
                logger.LogInformation("Confirmation email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send confirmation email to {Email}", email);
                throw;
            }
        }
    }
}