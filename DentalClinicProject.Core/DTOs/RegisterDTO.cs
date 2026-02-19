using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.DTOs
{
    public record RegisterDTO
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public Role Role { get; set; } = Role.User;
    }
}