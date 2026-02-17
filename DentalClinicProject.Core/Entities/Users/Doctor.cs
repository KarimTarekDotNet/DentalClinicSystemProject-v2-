using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicProject.Core.Entities.Users
{
    public class Doctor : BaseEntity
    {
        public decimal Salary { get; set; } = 0.0m;
        public int CapactiyOfDay { get; set; }
        public bool IsApproved { get; set; } = false;
        public string? ReasonForRejection { get; set; }
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = null!;

        [NotMapped]
        public int AppointmentsCount => Appointments.Count;
        [NotMapped]
        public bool IsFull => Appointments.Count == CapactiyOfDay;
    }
}