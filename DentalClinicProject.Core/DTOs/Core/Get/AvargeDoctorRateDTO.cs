namespace DentalClinicProject.Core.DTOs.Core.Get
{
    public record AvargeDoctorRateDTO
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = null!;
        public double AverageRate { get; set; }
        public string? Value { get; set; }
        public List<string?>? Comments { get; set; }
    }
}