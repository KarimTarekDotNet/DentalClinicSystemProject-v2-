using DentalClinicProject.Core.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicProject.Core.Entities.AuthModel
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } = false;
        public bool IsUsed { get; set; } = false;
        public string? CreatedByIp { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }


        [NotMapped]
        public bool IsActive => !IsRevoked && !IsUsed && DateTime.UtcNow <= ExpiryDate;

        // Mark token as used
        public void MarkAsUsed()
        {
            if (IsRevoked) throw new InvalidOperationException("Cannot use a revoked token.");
            if (IsUsed) throw new InvalidOperationException("Token is already used.");

            IsUsed = true;
        }

        // Revoke the token
        public void Revoke(string? ipAddress = null, string? replacedByToken = null)
        {
            if (IsRevoked)
                throw new InvalidOperationException("Token is already revoked.");

            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
            RevokedByIp = ipAddress ?? "Unknown";
            ReplacedByToken = replacedByToken;
        }
    }
}