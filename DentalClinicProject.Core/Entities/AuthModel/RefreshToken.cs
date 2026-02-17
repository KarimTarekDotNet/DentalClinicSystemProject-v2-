using DentalClinicProject.Core.Entities.Users;

namespace DentalClinicProject.Core.Entities.AuthModel
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } = false;
    }
}