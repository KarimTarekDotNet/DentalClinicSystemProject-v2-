using DentalClinicProject.Core.Entities.Core;

namespace DentalClinicProject.Core.Entities.Users
{
    public class Patient : BaseEntity
    {
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public int? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
    }
}