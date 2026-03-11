namespace DentalClinicProject.Core.DTOs
{
    public record RefreshTokenDTO
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string? UserId { get; set; } // Optional
    }
}
