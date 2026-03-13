namespace DentalClinicProject.Core.DTOs.Core.Update
{
    public class UpdateServiceDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? DurationInMinutes { get; set; }
    }
}
