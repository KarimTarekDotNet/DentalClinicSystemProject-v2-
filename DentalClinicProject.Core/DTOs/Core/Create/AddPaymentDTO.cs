using DentalClinicProject.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DentalClinicProject.Core.DTOs.Core.Create
{
    public record AddPaymentDTO
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public PaymentMethod PaymentMethod { get; set; }
    }
}
