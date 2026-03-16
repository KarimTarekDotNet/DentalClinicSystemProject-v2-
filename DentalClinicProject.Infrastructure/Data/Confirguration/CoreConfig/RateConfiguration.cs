using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.CoreConfig
{
    public class DoctorRateConfiguration : IEntityTypeConfiguration<DoctorRate>
    {
        public void Configure(EntityTypeBuilder<DoctorRate> builder)
        {
            builder.HasData(new DoctorRate
            {
                Id = SeedData.Rate2Id,
                CreatedAt = SeedData.Rate1CreatedDate,
                DoctorId = SeedData.Doctor1EntityId,
                Comment = SeedData.Rate1Comment,
                Value = RatingCategory.Excellent,
                UserId = SeedData.Patient1UserId
            });
        }
    }

    public class ProductRateConfiguration : IEntityTypeConfiguration<ProductRate>
    {
        public void Configure(EntityTypeBuilder<ProductRate> builder)
        {
            builder.HasData(new ProductRate
            {
                Id = SeedData.Rate1Id,
                CreatedAt = SeedData.Rate1CreatedDate,
                ProductId = SeedData.Product1Id,
                Comment = SeedData.Rate2Comment,
                Value = RatingCategory.Outstanding,
                UserId = SeedData.Patient1UserId
            });
        }
    }
    public class AppointmentRateConfiguration : IEntityTypeConfiguration<AppointmentRate>
    {
        public void Configure(EntityTypeBuilder<AppointmentRate> builder)
        {
            builder.HasData(new AppointmentRate
            {
                Id = 3,
                CreatedAt = SeedData.Rate1CreatedDate,
                AppointmentId = SeedData.Appointment1Id,
                Comment = SeedData.Rate2Comment,
                Value = RatingCategory.Outstanding,
                UserId = SeedData.Patient1UserId
            });
        }
    }
}