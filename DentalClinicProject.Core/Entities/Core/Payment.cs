    using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.Entities.Core
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string Description { get; set; } = null!;

        public DateTime? PaidAt { get; set; }

        public PaymentStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public string CustomerId { get; set; } = null!;

        public string? TransactionId { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}