namespace DentalClinicProject.Core.Entities.Core
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public List<CartItem> CartItems { get; set; } = new();
        public int Stock { get; set; }
    }
}