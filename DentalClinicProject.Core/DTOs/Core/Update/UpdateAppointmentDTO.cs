namespace DentalClinicProject.Core.DTOs.Core.Update
{
    public class UpdateAppointmentDTO
    {
        public int Id { get; set; }
        public int? DoctorId { get; set; }
        public int? ServiceId { get; set; }
        public DateTime? ExaminationAppointment { get; set; }
    }
}
