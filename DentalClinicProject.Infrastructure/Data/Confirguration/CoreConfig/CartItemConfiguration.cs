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
            builder.HasData(

                new CartItem
                {
                    Id = 1,
                    CartId = SeedData.CartItem1Id,
                    ProductId = SeedData.Product1Id,
                    Quantity = 1,
                    UnitPrice = SeedData.Product1Price
                },

                new CartItem
                {
                    Id = 2,
                    CartId = SeedData.CartItem1Id,
                    ProductId = SeedData.Product2Id,
                    Quantity = 1,
                    UnitPrice = SeedData.Product2Price
                },

                new CartItem
                {
                    Id = 3,
                    CartId = SeedData.CartItem2Id,
                    ProductId = SeedData.Product3Id,
                    Quantity = 1,
                    UnitPrice = SeedData.Product3Price
                },

                new CartItem
                {
                    Id = 4,
                    CartId = SeedData.CartItem2Id,
                    ProductId = SeedData.Product4Id,
                    Quantity = 1,
                    UnitPrice = SeedData.Product4Price
                }

            );
        }
    }
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasData(
                new Cart
                {
                    Id = SeedData.CartItem1Id,
                    UserId = SeedData.Patient1UserId,
                    CreatedAt = SeedData.CartItem1CreatedDate
                },
                new Cart
                {
                    Id = SeedData.CartItem2Id,
                    UserId = SeedData.RegularUserId,
                    CreatedAt = SeedData.CartItem2CreatedDate
                }
            );
        }
    }
}