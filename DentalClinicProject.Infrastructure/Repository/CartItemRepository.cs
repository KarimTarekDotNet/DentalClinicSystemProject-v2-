using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ApplicationDbContext _context;

        public CartItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<bool> AddProductToCartAsync(int cartItemId, int productId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ClearCartAsync(int cartItemId)
        {
            throw new NotImplementedException();
        }

        public Task<CartItem> CreateCartItemAsync(CreateCartItemDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<CartItem>> GetCartItemsWithProductsAsync(PaginationParams param)
        {
            throw new NotImplementedException();
        }

        public Task<CartItem?> GetCartItemWithProductsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveProductFromCartAsync(int cartItemId, int productId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCartItemAsync(int id, UpdateCartItemDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
