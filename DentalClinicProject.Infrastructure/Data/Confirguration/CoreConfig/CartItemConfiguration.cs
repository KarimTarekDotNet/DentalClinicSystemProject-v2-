using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalClinicProject.Infrastructure.Data.Confirguration.CoreConfig
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasData(new CartItem
            {
                Id = SeedData.CartItem1Id,
                CreatedAt = SeedData.CartItem1CreatedDate,
                TotalPrice = SeedData.CartItem1TotalPrice,
                ItemCount = SeedData.CartItem1ItemCount
            });
            builder.HasData(new CartItem
            {
                Id = SeedData.CartItem2Id,
                CreatedAt = SeedData.CartItem2CreatedDate,
                TotalPrice = SeedData.CartItem2TotalPrice,
                ItemCount = SeedData.CartItem2ItemCount
            });
        }
    }
}
