using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.Entities.Core
{
    public abstract class Rate : BaseEntity
    {
        public string UserId { get; set; } = null!;
        public RatingCategory Value { get; set; }
        public string? Comment { get; set; }
    }

    public class DoctorRate : Rate
    {
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;
    }

    public class ProductRate : Rate
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }

    public class AppointmentRate : Rate
    {
        public int AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; } = null!;
    }
}