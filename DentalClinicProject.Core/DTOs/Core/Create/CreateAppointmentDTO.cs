namespace DentalClinicProject.Core.DTOs.Core.Create
{
    public class CreateAppointmentDTO
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ServiceId { get; set; }
        public DateTime ExaminationAppointment { get; set; }
    }
}
