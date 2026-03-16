using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.DTOs.Core.Create
{
    public record CreateRateDTO
    {
        public RatingCategory Value { get; set; }
        public string? Comment { get; set; }
    }
    public record CreateDoctorRateDTO : CreateRateDTO
    {
        public int DoctorId { get; set; }
    }
    public record CreateProductRateDTO : CreateRateDTO
    {
        public int ProductId { get; set; }
    }
    public record CreateApponitmentRateDTO : CreateRateDTO
    {
        public int AppointmentId { get; set; }
    }
}
