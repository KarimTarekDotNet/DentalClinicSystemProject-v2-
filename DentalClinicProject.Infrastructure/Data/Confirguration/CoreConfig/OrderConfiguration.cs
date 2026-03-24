using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.CoreConfig
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.Status)
                .HasConversion<string>();


            builder.HasData(
                new Order
                {
                    Id = SeedData.Order1Id,
                    DeliveryId = SeedData.Doctor1EntityId,
                    Status = SeedData.Order1Status,
                    UserId = SeedData.RegularUserId,
                    DeliveryDate = new(2026, 4, 5, 12, 0, 0)
                },
                new Order
                {
                    Id = SeedData.Order2Id,
                    DeliveryId = SeedData.Doctor2EntityId,
                    Status = SeedData.Order2Status,
                    UserId = SeedData.RegularUserId,
                    DeliveryDate = new(2026, 4, 6, 12, 0, 0)
                }
            );
        }
    }
}