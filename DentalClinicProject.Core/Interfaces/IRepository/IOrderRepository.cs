using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IOrderRepository
    {
        Task<List<OrderDTO>> GetOrdersForDeliveryAsync(int deliveryId, DateTime deliveryDate);
        Task<OrderDTO?> GetOrderByIdAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task AddPaymentAsync(int orderId, AddPaymentDTO paymentDto);
        Task<OrderDTO> CreateOrderAsync(CreateOrderDTO dto, string userId);
        Task CancelOrderAsync(int orderId);
        Task<List<OrderDTO>> GetOrdersByUserAsync(string userId); 
        Task UpdateOrderItemsAsync(int orderId, List<CreateOrderItemDTO> newItems);
    }
}