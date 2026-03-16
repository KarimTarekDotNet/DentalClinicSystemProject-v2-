namespace DentalClinicProject.Core.DTOs.Core.Get
{
    public record RateDTO
    {
        public int Id { get; set; }
        public string Value { get; set; } = null!;
        public string? Comment { get; set; }
    }
    public record RateDoctorDTO : RateDTO
    {
        public int? DoctorId { get; set; }
        public string? DoctorName { get; set; }
    }
    public record RateProductDTO : RateDTO
    {
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
    }
    public record RateAppointmentDTO : RateDTO
    {
        public int? AppointmentId { get; set; }
    }
}