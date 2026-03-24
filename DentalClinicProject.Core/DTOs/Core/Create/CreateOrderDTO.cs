using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinicProject.Core.DTOs.Core.Create
{
    public class CreateOrderDTO
    {
        public int DeliveryId { get; set; }

        public DateTime DeliveryDate { get; set; }

        public List<CreateOrderItemDTO> Items { get; set; } = new();
    }
    public class CreateOrderItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
