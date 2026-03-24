using DentalClinicProject.Core.Entities.Core;
using System.Text.Json.Serialization;

namespace DentalClinicProject.Core.DTOs.Core.Get
{
    public record CartProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal Total => UnitPrice * Quantity;
    }

    public record CartDTO
    {
        public int Id { get; set; }
        public List<CartProductDTO> Items { get; set; } = new();

        public decimal TotalPrice { get; set; }

        public int TotalItems { get; set; }
    }
}