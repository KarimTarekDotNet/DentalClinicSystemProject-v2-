namespace DentalClinicProject.Core.DTOs.Auth
{
    public record VerifyLoginCodeDTO
    {
        public string Identifier { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
