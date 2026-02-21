using Microsoft.AspNetCore.Http;

namespace DentalClinicProject.Core.Interfaces.IServices
{
    public interface IMailService
    {
        Task SendConfirmEmail(string to, string subject, string message, IList<IFormFile>? Attachments = null);
        Task SendConfirmationEmail(string email, string subject, string body);
        Task SendVerificationCodeEmail(string email, string code);
    }
}
