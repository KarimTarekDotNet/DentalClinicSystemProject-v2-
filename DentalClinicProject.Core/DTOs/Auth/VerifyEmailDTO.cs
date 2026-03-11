namespace DentalClinicProject.Core.DTOs.Auth
{
    public class VerifyEmailDTO
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
