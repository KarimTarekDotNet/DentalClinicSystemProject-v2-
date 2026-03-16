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
            builder.HasData(

                new Product
                {
                    Id = SeedData.Product1Id,
                    Name = SeedData.Product1Name,
                    Description = SeedData.Product1Description,
                    Price = SeedData.Product1Price,
                    CreatedAt = new(2024, 1, 25)
                },

                new Product
                {
                    Id = SeedData.Product2Id,
                    Name = SeedData.Product2Name,
                    Description = SeedData.Product2Description,
                    Price = SeedData.Product2Price,
                    CreatedAt = new(2024, 1, 25)
                },

                new Product
                {
                    Id = SeedData.Product3Id,
                    Name = SeedData.Product3Name,
                    Description = SeedData.Product3Description,
                    Price = SeedData.Product3Price,
                    CreatedAt = new(2024, 1, 25)
                },

                new Product
                {
                    Id = SeedData.Product4Id,
                    Name = SeedData.Product4Name,
                    Description = SeedData.Product4Description,
                    Price = SeedData.Product4Price,
                    CreatedAt = new(2024, 1, 25)
                }

            );
        }
    }
}