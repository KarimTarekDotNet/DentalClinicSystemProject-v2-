namespace DentalClinicProject.Core.DTOs.Auth
{
    public record VerifyPhoneDTO
    {
        public string Code { get; set; } = null!;
    }
}
