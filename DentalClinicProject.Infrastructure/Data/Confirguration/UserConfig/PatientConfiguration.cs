using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.UserConfig
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasData(new Patient
            {
                Id = SeedData.Patient1EntityId,
                AppUserId = SeedData.Patient1UserId,
                CreatedAt = SeedData.Patient1CreatedDate,
                DoctorId = SeedData.Doctor1EntityId
            });
            builder.HasData(new Patient
            {
                Id = SeedData.Patient2EntityId,
                AppUserId = SeedData.Patient2UserId,
                CreatedAt = SeedData.Patient2CreatedDate,
                DoctorId = SeedData.Doctor1EntityId
            });
        }
    }
}