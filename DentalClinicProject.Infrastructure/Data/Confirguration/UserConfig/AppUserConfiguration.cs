using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.UserConfig
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(x => x.Provider)
                .HasConversion<string>();

            builder.HasData(

                new AppUser
                {
                    Id = SeedData.AdminUserId,
                    UserName = SeedData.AdminUserName,
                    NormalizedUserName = SeedData.AdminNormalizedUserName,
                    Email = SeedData.AdminEmail,
                    NormalizedEmail = SeedData.AdminNormalizedEmail,
                    FirstName = SeedData.AdminFirstName,
                    LastName = SeedData.AdminLastName,
                    PasswordHash = SeedData.AdminPasswordHash,
                    SecurityStamp = SeedData.AdminSecurityStamp,
                    ConcurrencyStamp = SeedData.AdminConcurrencyStamp,
                    ProviderId = SeedData.AdminProviderId,
                    EmailConfirmed = true
                },

                new AppUser
                {
                    Id = SeedData.Doctor1UserId,
                    UserName = SeedData.Doctor1UserName,
                    NormalizedUserName = SeedData.Doctor1NormalizedUserName,
                    Email = SeedData.Doctor1Email,
                    NormalizedEmail = SeedData.Doctor1NormalizedEmail,
                    FirstName = SeedData.Doctor1FirstName,
                    LastName = SeedData.Doctor1LastName,
                    PasswordHash = SeedData.Doctor1PasswordHash,
                    SecurityStamp = SeedData.Doctor1SecurityStamp,
                    ConcurrencyStamp = SeedData.Doctor1ConcurrencyStamp,
                    ProviderId = SeedData.Doctor1ProviderId,
                    EmailConfirmed = true
                },

                new AppUser
                {
                    Id = SeedData.Doctor2UserId,
                    UserName = SeedData.Doctor2UserName,
                    NormalizedUserName = SeedData.Doctor2NormalizedUserName,
                    Email = SeedData.Doctor2Email,
                    NormalizedEmail = SeedData.Doctor2NormalizedEmail,
                    FirstName = SeedData.Doctor2FirstName,
                    LastName = SeedData.Doctor2LastName,
                    PasswordHash = SeedData.Doctor2PasswordHash,
                    SecurityStamp = SeedData.Doctor2SecurityStamp,
                    ConcurrencyStamp = SeedData.Doctor2ConcurrencyStamp,
                    ProviderId = SeedData.Doctor2ProviderId,
                    EmailConfirmed = true
                },

                new AppUser
                {
                    Id = SeedData.Patient1UserId,
                    UserName = SeedData.Patient1UserName,
                    NormalizedUserName = SeedData.Patient1NormalizedUserName,
                    Email = SeedData.Patient1Email,
                    NormalizedEmail = SeedData.Patient1NormalizedEmail,
                    FirstName = SeedData.Patient1FirstName,
                    LastName = SeedData.Patient1LastName,
                    PasswordHash = SeedData.Patient1PasswordHash,
                    SecurityStamp = SeedData.Patient1SecurityStamp,
                    ConcurrencyStamp = SeedData.Patient1ConcurrencyStamp,
                    ProviderId = SeedData.Patient1ProviderId,
                    EmailConfirmed = true
                },

                new AppUser
                {
                    Id = SeedData.Patient2UserId,
                    UserName = SeedData.Patient2UserName,
                    NormalizedUserName = SeedData.Patient2NormalizedUserName,
                    Email = SeedData.Patient2Email,
                    NormalizedEmail = SeedData.Patient2NormalizedEmail,
                    FirstName = SeedData.Patient2FirstName,
                    LastName = SeedData.Patient2LastName,
                    PasswordHash = SeedData.Patient2PasswordHash,
                    SecurityStamp = SeedData.Patient2SecurityStamp,
                    ConcurrencyStamp = SeedData.Patient2ConcurrencyStamp,
                    ProviderId = SeedData.Patient2ProviderId,
                    EmailConfirmed = true
                },

                new AppUser
                {
                    Id = SeedData.RegularUserId,
                    UserName = SeedData.RegularUserName,
                    NormalizedUserName = SeedData.RegularNormalizedUserName,
                    Email = SeedData.RegularEmail,
                    NormalizedEmail = SeedData.RegularNormalizedEmail,
                    FirstName = SeedData.RegularFirstName,
                    LastName = SeedData.RegularLastName,
                    PasswordHash = SeedData.RegularPasswordHash,
                    SecurityStamp = SeedData.RegularSecurityStamp,
                    ConcurrencyStamp = SeedData.RegularConcurrencyStamp,
                    ProviderId = SeedData.RegularProviderId,
                    EmailConfirmed = true
                }
            );
        }
    }
}