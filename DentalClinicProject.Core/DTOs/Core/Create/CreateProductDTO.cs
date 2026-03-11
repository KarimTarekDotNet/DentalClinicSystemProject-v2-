namespace DentalClinicProject.Core.DTOs.Core.Create
{
    public class CreateProductDTO
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
