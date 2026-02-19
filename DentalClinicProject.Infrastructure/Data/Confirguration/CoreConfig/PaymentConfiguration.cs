using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.CoreConfig
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(x => x.Status)
                .HasConversion<string>();
            builder.HasData(new Payment
            {
                Id = SeedData.Payment1Id,
                Amount = SeedData.Payment1Amount,
                Currency = SeedData.Payment1Currency,
                CustomerId = SeedData.RegularUserId,
                Description = SeedData.Payment1Description,
                PaymentDate = SeedData.Payment1Date,
                PaymentMethod = SeedData.Payment1Method,
                Status = PaymentStatus.Paid,
                TransactionId = SeedData.Payment1TransactionId,
                ProductId = SeedData.Product1Id,
            });
            builder.HasData(new Payment
            {
                Id = SeedData.Payment2Id,
                Amount = SeedData.Payment2Amount,
                Currency = SeedData.Payment2Currency,
                CustomerId = SeedData.RegularUserId,
                Description = SeedData.Payment2Description,
                PaymentDate = SeedData.Payment2Date,
                PaymentMethod = SeedData.Payment2Method,
                Status = PaymentStatus.Refunded,
                TransactionId = SeedData.Payment2TransactionId,
                ProductId = SeedData.Product2Id,
            });
        }
    }
}