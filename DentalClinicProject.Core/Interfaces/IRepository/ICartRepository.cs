using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.Enum;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface ICartRepository
    {
        Task<CartDTO?> GetCartByUserIdAsync(string userId, Role role);
        Task<bool> AddProductToCartAsync(int productId, string userId, Role role, int quantity = 1);
        Task<bool> RemoveProductFromCartAsync(int cartId, int productId, string userId, Role role, int quantity = 1);
        Task<bool> ClearCartAsync(int cartId, string userId, Role role);
    }
}