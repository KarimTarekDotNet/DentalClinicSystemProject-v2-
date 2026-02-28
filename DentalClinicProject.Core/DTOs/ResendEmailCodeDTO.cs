namespace DentalClinicProject.Core.DTOs
{
    public record ResendEmailCodeDTO
    {
        public string SessionToken { get; set; } = null!;
    }
}
