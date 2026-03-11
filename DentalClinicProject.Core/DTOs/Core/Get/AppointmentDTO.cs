namespace DentalClinicProject.Core.DTOs.Core.Get
{
    public record AppointmentDTO
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = null!;
        public string DoctorName { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public DateTime ExaminationEppointment { get; set; }
    }
}