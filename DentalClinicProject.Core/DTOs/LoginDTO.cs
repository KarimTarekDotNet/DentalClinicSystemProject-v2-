namespace DentalClinicProject.Core.DTOs
{
    public record LoginDTO
    {
        // Can be Email, Username, or PhoneNumber
        public string Identifier { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}