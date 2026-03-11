using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface ICartItemRepository
    {
        Task<CartItem?> GetCartItemWithProductsAsync(int id);
        Task<PagedResult<CartItem>> GetCartItemsWithProductsAsync(PaginationParams param);
        Task<CartItem> CreateCartItemAsync(CreateCartItemDTO dto);
        Task<bool> UpdateCartItemAsync(int id, UpdateCartItemDTO dto);
        Task<bool> AddProductToCartAsync(int cartItemId, int productId);
        Task<bool> RemoveProductFromCartAsync(int cartItemId, int productId);
        Task<bool> ClearCartAsync(int cartItemId);
    }
}
