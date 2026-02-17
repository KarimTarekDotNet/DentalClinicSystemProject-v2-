using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.Entities.Core
{
    public class Rate : BaseEntity
    {
        public RatingCategory Value { get; set; }
        public string? Comment { get; set; }

        public Appointment Appointment { get; set; } = null!;
        public int AppointmentId { get; set; }
        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public int DoctorId { get; set; }
    }
}