namespace DentalClinicProject.Core.DTOs
{
    public record ExternalLoginRequest
    {
        public string Provider { get; set; } = null!;
        public string ProviderKey { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
