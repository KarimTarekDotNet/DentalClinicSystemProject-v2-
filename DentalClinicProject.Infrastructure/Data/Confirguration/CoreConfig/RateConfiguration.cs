using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.CoreConfig
{
    public class RateConfiguration : IEntityTypeConfiguration<Rate>
    {
        public void Configure(EntityTypeBuilder<Rate> builder)
        {
            builder.HasData(new Rate
            {
                Id = SeedData.Product1Id,
                CreatedAt = SeedData.Rate1CreatedDate,
                AppointmentId = SeedData.Appointment1Id,
                Comment = SeedData.Rate1Comment,
                DoctorId = SeedData.Doctor1EntityId,
                ProductId = SeedData.Product1Id,
                Value = RatingCategory.Excellent
            });
            builder.HasData(new Rate
            {
                Id = SeedData.Product2Id,
                CreatedAt = SeedData.Rate2CreatedDate,
                AppointmentId = SeedData.Appointment2Id,
                Comment = SeedData.Rate2Comment,
                DoctorId = SeedData.Doctor2EntityId,
                ProductId = SeedData.Product2Id,
                Value = RatingCategory.Good
            });
        }
    }
}