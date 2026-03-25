using DentalClinicProject.Core.Entities.Core;
namespace DentalClinicProject.Core.DTOs.Core.Create
{
    public class CreateOrderItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
