using DentalClinicProject.Core.Entities.Users;

namespace DentalClinicProject.Core.Entities.Core
{
    public class CartItem : BaseEntity
    {
        public List<Product> Products { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public int ItemCount { get; set; }
    }
}