using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.CoreConfig
{
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