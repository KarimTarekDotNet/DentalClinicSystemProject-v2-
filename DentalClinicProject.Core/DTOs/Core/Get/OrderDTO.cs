namespace DentalClinicProject.Core.DTOs.Core.Get
{
    public record OrderDTO
    {
        public int Id { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string UserId { get; set; } = null!;
        public List<OrderItemDTO> Items { get; set; } = new();
        public List<PaymentDTO> Payments { get; set; } = new();
    }
    public record OrderItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
    public record PaymentDTO
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public DateTime? PaidAt { get; set; }
        public string? TransactionId { get; set; }
    }
}
