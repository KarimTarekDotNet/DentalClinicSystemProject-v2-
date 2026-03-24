namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IDeliveryRepository
    {
        Task AssignOrdersAsync(List<int> orderIds, DateTime deliveryDate, string deliveryId);
        Task<bool> DeliverOrderAsync(int orderId, decimal? cashReceived = null);
        Task<Dictionary<int, string>> TrackAssignedOrdersAsync();
        Task<List<int>> GetPendingOrdersAsync(DateTime deliveryDate);
    }
}