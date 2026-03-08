namespace DentalClinicProject.Core.DTOs
{
    public class ExternalLoginCallbackDTO
    {
        public string Provider { get; set; } = null!;
        public string ProviderKey { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
}
