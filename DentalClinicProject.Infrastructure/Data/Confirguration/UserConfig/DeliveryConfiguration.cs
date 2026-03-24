using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.UserConfig
{
    public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.HasData(new Delivery
            {
                Id = SeedData.Doctor1EntityId,
                AppUserId = SeedData.Delivery1UserId,
                CreatedAt = SeedData.Doctor1CreatedDate,
                CapactiyOfDay = SeedData.Doctor1Capacity,
                IsApproved = true,
                ReasonForRejection = null,
                Salary = SeedData.Doctor1Salary
            });
            builder.HasData(new Delivery
            {
                Id = SeedData.Doctor2EntityId,
                AppUserId = SeedData.Delivery2UserId,
                CreatedAt = SeedData.Doctor2CreatedDate,
                CapactiyOfDay = SeedData.Doctor2Capacity,
                IsApproved = false,
                ReasonForRejection = "He did not submit the required documents."
            });
        }
    }
}