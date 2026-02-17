using DentalClinicProject.Core.Enum;
using Microsoft.AspNetCore.Identity;

namespace DentalClinicProject.Core.Entities.Users
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Provider Provider { get; set; } = Provider.Local;
        public string ProviderId { get; set; } = Guid.CreateVersion7().ToString();
    }
}