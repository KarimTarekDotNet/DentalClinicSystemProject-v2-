using DentalClinicProject.Core.Entities.Users;

namespace DentalClinicProject.Core.Entities.Core
{
    public class Appointment : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        public DateTime ExaminationEppointment { get; set; }
    }
}