namespace DentalClinicProject.Core.DTOs.Auth
{
    public record ResendEmailCodeDTO
    {
        public string SessionToken { get; set; } = null!;
    }
}
