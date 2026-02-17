using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.Entities.Core
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
        public string CustomerId { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }
    }
}