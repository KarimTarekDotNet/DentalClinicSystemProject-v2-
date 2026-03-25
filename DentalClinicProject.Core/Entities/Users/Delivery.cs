using DentalClinicProject.Core.Entities.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicProject.Core.Entities.Users
{
    public class Delivery : BaseEntity
    {
        public decimal Salary { get; set; } = 0.0m;
        public bool IsApproved { get; set; } = false;
        public string? ReasonForRejection { get; set; }
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = null!;
    }
}
