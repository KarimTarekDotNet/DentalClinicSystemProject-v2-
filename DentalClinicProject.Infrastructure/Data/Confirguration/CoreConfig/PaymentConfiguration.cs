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

            builder.Property(x => x.PaymentMethod)
                .HasConversion<string>();

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.HasData(
                new Payment
                {
                    Id = SeedData.Payment1Id,
                    Amount = SeedData.Payment1Amount,
                    Currency = SeedData.Payment1Currency,
                    CustomerId = SeedData.RegularUserId,
                    Description = SeedData.Payment1Description,
                    PaidAt = SeedData.Payment1Date,
                    PaymentMethod = SeedData.Payment1Method,
                    Status = PaymentStatus.Paid,
                    TransactionId = SeedData.Payment1TransactionId,
                    OrderId = SeedData.Order1Id,
                },
                new Payment
                {
                    Id = SeedData.Payment2Id,
                    Amount = SeedData.Payment2Amount,
                    Currency = SeedData.Payment2Currency,
                    CustomerId = SeedData.RegularUserId,
                    Description = SeedData.Payment2Description,
                    PaidAt = SeedData.Payment2Date,
                    PaymentMethod = SeedData.Payment2Method,
                    Status = PaymentStatus.Refunded,
                    TransactionId = SeedData.Payment2TransactionId,
                    OrderId = SeedData.Order2Id,
                }
            );
        }
    }

    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.Status)
                .HasConversion<string>();

            // TotalAmount is [NotMapped] - no column config needed

            builder.HasData(
                new
                {
                    Id = SeedData.Order1Id,
                    Status = SeedData.Order1Status,
                    UserId = SeedData.RegularUserId,
                },
                new
                {
                    Id = SeedData.Order2Id,
                    Status = SeedData.Order2Status,
                    UserId = SeedData.RegularUserId,
                }
            );
        }
    }

    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(x => x.Price)
                .HasPrecision(18, 2);

            builder.HasData(
                new OrderItem
                {
                    Id = SeedData.OrderItem1Id,
                    ProductId = SeedData.Product1Id,
                    ProductName = SeedData.OrderItem1ProductName,
                    Price = SeedData.OrderItem1Price,
                    Quantity = SeedData.OrderItem1Quantity,
                    OrderId = SeedData.Order1Id,
                },
                new OrderItem
                {
                    Id = SeedData.OrderItem2Id,
                    ProductId = SeedData.Product2Id,
                    ProductName = SeedData.OrderItem2ProductName,
                    Price = SeedData.OrderItem2Price,
                    Quantity = SeedData.OrderItem2Quantity,
                    OrderId = SeedData.Order2Id,
                },
                new OrderItem
                {
                    Id = SeedData.OrderItem3Id,
                    ProductId = SeedData.Product3Id,
                    ProductName = SeedData.OrderItem3ProductName,
                    Price = SeedData.OrderItem3Price,
                    Quantity = SeedData.OrderItem3Quantity,
                    OrderId = SeedData.Order2Id,
                }
            );
        }
    }
}