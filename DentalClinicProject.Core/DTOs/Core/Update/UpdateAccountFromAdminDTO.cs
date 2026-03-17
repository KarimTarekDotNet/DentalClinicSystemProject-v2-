using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.DTOs.Core.Update
{
    public record UpdateAccountFromAdminDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Role? Role { get; set; }
    }
    public record DeleteAccountFromAdminDTO
    {
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
    }
}