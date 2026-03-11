using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.DTOs.Core.Create
{
    public class CreateRateDTO
    {
        public RatingCategory Value { get; set; }
        public string? Comment { get; set; }
        public int AppointmentId { get; set; }
        public int ProductId { get; set; }
        public int DoctorId { get; set; }
    }
}
