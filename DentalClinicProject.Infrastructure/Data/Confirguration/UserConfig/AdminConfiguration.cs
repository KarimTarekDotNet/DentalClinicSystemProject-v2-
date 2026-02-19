using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.UserConfig
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasData(new Admin
            {
                Id = SeedData.AdminEntityId,
                AppUserId = SeedData.AdminUserId,
                CreatedAt = SeedData.AdminCreatedDate
            });
        }
    }
}