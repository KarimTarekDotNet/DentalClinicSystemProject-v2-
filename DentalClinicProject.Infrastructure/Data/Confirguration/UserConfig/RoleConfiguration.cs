using DentalClinicProject.Core.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.UserConfig
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = "role-admin",
                    Name = SeedData.AdminRole,
                    NormalizedName = SeedData.AdminNormalizedRole,
                    ConcurrencyStamp = "ROLE-ADMIN-001"
                },
                new IdentityRole
                {
                    Id = "role-doctor",
                    Name = SeedData.DoctorRole,
                    NormalizedName = SeedData.DoctorNormalizedRole,
                    ConcurrencyStamp = "ROLE-DOCTOR-001"
                },
                new IdentityRole
                {
                    Id = "role-patient",
                    Name = SeedData.PatientRole,
                    NormalizedName = SeedData.PatientNormalizedRole,
                    ConcurrencyStamp = "ROLE-PATIENT-001"
                },
                new IdentityRole
                {
                    Id = "role-user",
                    Name = SeedData.UserRole,
                    NormalizedName = SeedData.UserNormalizedRole,
                    ConcurrencyStamp = "ROLE-USER-001"
                }
            );
        }
    }
}