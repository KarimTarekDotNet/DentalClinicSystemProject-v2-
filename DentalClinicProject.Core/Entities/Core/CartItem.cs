using DentalClinicProject.Core.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicProject.Core.Entities.Core
{
    public class CartItem : BaseEntity
    {
        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }

    public class Cart : BaseEntity
    {
        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;

        public List<CartItem> Items { get; set; } = new();

        [NotMapped]
        public decimal TotalPrice => Items.Sum(i => i.UnitPrice * i.Quantity);
    }
}