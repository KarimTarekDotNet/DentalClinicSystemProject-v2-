using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.UserConfig
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasData(new Doctor
            {
                Id = SeedData.Doctor1EntityId,
                AppUserId = SeedData.Doctor1UserId,
                CreatedAt = SeedData.Doctor1CreatedDate,
                CapactiyOfDay = SeedData.Doctor1Capacity,
                IsApproved = true,
                ReasonForRejection = null,
                Salary = SeedData.Doctor1Salary
            });
            builder.HasData(new Doctor
            {
                Id = SeedData.Doctor2EntityId,
                AppUserId = SeedData.Doctor2UserId,
                CreatedAt = SeedData.Doctor2CreatedDate,
                CapactiyOfDay = SeedData.Doctor2Capacity,
                IsApproved = false,
                ReasonForRejection = "He did not submit the required documents."
            });
        }
    }
}