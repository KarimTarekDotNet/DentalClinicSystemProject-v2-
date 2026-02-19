using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.CoreConfig
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasData(new Service
            {
                Id = SeedData.Service1Id,
                CreatedAt = SeedData.Rate1CreatedDate,
                DurationInMinutes = SeedData.Service1Duration,
                Name = SeedData.Service1Name,
                Price = SeedData.Service1Price
            });
            builder.HasData(new Service
            {
                Id = SeedData.Service2Id,
                CreatedAt = SeedData.Rate2CreatedDate,
                DurationInMinutes = SeedData.Service2Duration,
                Name = SeedData.Service2Name,
                Price = SeedData.Service2Price
            });
            builder.HasData(new Service
            {
                Id = SeedData.Service3Id,
                CreatedAt = SeedData.Rate2CreatedDate,
                DurationInMinutes = SeedData.Service3Duration,
                Name = SeedData.Service3Name,
                Price = SeedData.Service3Price
            });
            builder.HasData(new Service
            {
                Id = SeedData.Service4Id,
                CreatedAt = SeedData.Rate2CreatedDate,
                DurationInMinutes = SeedData.Service4Duration,
                Name = SeedData.Service4Name,
                Price = SeedData.Service4Price
            });
            builder.HasData(new Service
            {
                Id = SeedData.Service5Id,
                CreatedAt = SeedData.Rate2CreatedDate,
                DurationInMinutes = SeedData.Service5Duration,
                Name = SeedData.Service5Name,
                Price = SeedData.Service5Price
            });
        }
    }
}