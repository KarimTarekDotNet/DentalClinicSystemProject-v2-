namespace DentalClinicProject.Core.Interfaces.IServices
{
    public interface IPhoneService
    {
        Task SendCodeAsync(string phone);
        Task<bool> VerifyCodeAsync(string phone, string code);
    }
}
