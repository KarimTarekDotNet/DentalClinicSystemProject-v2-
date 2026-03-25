using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IOrderRepository
    {
        Task<List<OrderDTO>> GetOrdersForDeliveryAsync(int deliveryId, DateTime deliveryDate);
        Task<OrderDTO?> GetOrderByIdAsync(int orderId);
        Task<List<OrderDTO>> GetOrdersByUserAsync(string userId);
        Task<OrderDTO> CreateOrderAsync(List<CreateOrderItemDTO> Items, string userId);
        Task CancelOrderAsync(int orderId);
        Task UpdateOrderItemsAsync(int orderId, List<CreateOrderItemDTO> newItems);
        Task AddPaymentAsync(int orderId, AddPaymentDTO? paymentDto = null, bool isCashOnDelivery = false);
        Task MarkOrderAsShippedAsync(int orderId);
        Task MarkOrderAsOutForDeliveryAsync(int orderId);
        Task CompleteOrderAsync(int orderId);
        Task ConfirmPaymentAsync(int paymentId);
    }
}