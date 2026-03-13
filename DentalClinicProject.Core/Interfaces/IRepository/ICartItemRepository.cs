using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface ICartItemRepository
    {
        Task<CartItemDTO?> GetCartItemWithProductsAsync(int id, string currentUserId, Role role);

        Task<PagedResult<CartItemDTO>> GetCartItemsWithProductsAsync(string currentUserId, Role role, int pageNumber = 1, int pageSize = 10);

        Task<CartItemDTO> CreateCartItemAsync(CreateCartItemDTO dto, string currentUserId);

        Task<CartItemDTO> UpdateCartItemAsync(UpdateCartItemDTO dto, string currentUserId, Role role);

        Task<bool> AddProductToCartAsync(int cartItemId, int productId, string currentUserId, Role role);

        Task<bool> RemoveProductFromCartAsync(int cartItemId, int productId, string currentUserId, Role role);

        Task<bool> ClearCartAsync(int cartItemId, string currentUserId, Role role);
    }
}