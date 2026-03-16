namespace DentalClinicProject.Core.DTOs.Core.Get
{
    public record ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
