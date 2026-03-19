using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicProject.Core.Entities.Core
{
    public class Order
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }

        public AppUser AppUser { get; set; } = null!;
        public string UserId { get; set; } = null!;

        [NotMapped]
        public decimal TotalAmount => Items.Sum(i => i.Price * i.Quantity);

        public List<OrderItem> Items { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();
    }
    public class OrderItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string ProductName { get; set; } = null!;

        public decimal Price { get; set; } // snapshot
        public int Quantity { get; set; }

        public Order Order { get; set; } = null!;
        public int OrderId { get; set; }
    }
}