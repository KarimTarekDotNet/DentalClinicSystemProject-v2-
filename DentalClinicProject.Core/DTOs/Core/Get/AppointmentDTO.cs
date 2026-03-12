using System.Text.Json.Serialization;

namespace DentalClinicProject.Core.DTOs.Core.Get
{
    public record AppointmentDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        [JsonIgnore]
        public string PatientAppUserId { get; set; } = null!;
        public string PatientName { get; set; } = null!;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public DateTime ExaminationEppointment { get; set; }
    }
}