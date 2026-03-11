namespace DentalClinicProject.Core.DTOs.Core.Create
{
    public class CreateServiceDTO
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
