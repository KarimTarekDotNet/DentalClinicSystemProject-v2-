namespace DentalClinicProject.Core.DTOs.Core.Get
{
    public record AvargeProductRateDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public double AverageRate { get; set; }
        public string? Value { get; set; }
        public List<string?>? Comments { get; set; }
    }
}