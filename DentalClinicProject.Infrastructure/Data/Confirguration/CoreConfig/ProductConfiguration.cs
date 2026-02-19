using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.CoreConfig
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasData(new Product
            {
                Id = SeedData.Product1Id,
                CreatedAt = SeedData.Appointment1CreatedDate,
                Description = SeedData.Product1Description,
                Name = SeedData.Product1Name,
                Price = SeedData.Product1Price,
            });
            builder.HasData(new Product
            {
                Id = SeedData.Product2Id,
                CreatedAt = SeedData.Appointment2CreatedDate,
                Description = SeedData.Product2Description,
                Name = SeedData.Product2Name,
                Price = SeedData.Product2Price,
            });
            builder.HasData(new Product
            {
                Id = SeedData.Product3Id,
                CreatedAt = SeedData.Appointment2CreatedDate,
                Description = SeedData.Product3Description,
                Name = SeedData.Product3Name,
                Price = SeedData.Product3Price,
            });
            builder.HasData(new Product
            {
                Id = SeedData.Product4Id,
                CreatedAt = SeedData.Appointment2CreatedDate,
                Description = SeedData.Product4Description,
                Name = SeedData.Product4Name,
                Price = SeedData.Product4Price,
            });
        }
    }
}