using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DentalClinicProject.API.Controllers.Core
{
    [Authorize]
    public class CartItemController : BaseController
    {
        public CartItemController(IUnitOfWork work, IMapper mapper) : base(work, mapper) { }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        private Role GetUserRole() => Enum.Parse<Role>(User.FindFirstValue(ClaimTypes.Role)!);

        [HttpGet("get-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCartItemsWithProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var currentUserId = GetUserId();
                var role = GetUserRole();

                var result = await work.CartItemRepository.GetCartItemsWithProductsAsync(currentUserId, role, pageNumber, pageSize);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving carts.", error = ex.Message });
            }
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetCartItemWithProducts([FromQuery] int id)
        {
            try
            {
                var currentUserId = GetUserId();
                var role = GetUserRole();

                var result = await work.CartItemRepository.GetCartItemWithProductsAsync(id, currentUserId, role);
                if (result == null)
                    return NotFound(new { success = false, message = "Cart not found or access denied." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving cart.", error = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCartItemAsync(CreateCartItemDTO dto)
        {
            try
            {
                var currentUserId = GetUserId();
                var cartItem = await work.CartItemRepository.CreateCartItemAsync(dto, currentUserId);
                return Ok(new { success = true, data = cartItem });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while creating cart.", error = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItemAsync(UpdateCartItemDTO dto)
        {
            try
            {
                var currentUserId = GetUserId();
                var role = GetUserRole();

                var result = await work.CartItemRepository.UpdateCartItemAsync(dto, currentUserId, role);
                if (result == null) return NotFound(new { success = false, message = "Cart not found or access denied." });

                return Ok(new { success = true, message = "Cart updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating cart.", error = ex.Message });
            }
        }

        [HttpPost("add-product")]
        public async Task<IActionResult> AddProductToCart([FromQuery] int cartItemId, [FromQuery] int productId)
        {
            try
            {
                var currentUserId = GetUserId();
                var role = GetUserRole();

                var success = await work.CartItemRepository.AddProductToCartAsync(cartItemId, productId, currentUserId, role);
                if (!success) return NotFound(new { success = false, message = "Cart or product not found or access denied." });

                return Ok(new { success = true, message = "Product added to cart successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while adding product to cart.", error = ex.Message });
            }
        }

        [HttpDelete("clear-product")]
        public async Task<IActionResult> ClearCart([FromQuery] int cartItemId)
        {
            try
            {
                var currentUserId = GetUserId();
                var role = GetUserRole();

                var success = await work.CartItemRepository.ClearCartAsync(cartItemId, currentUserId, role);
                if (!success) return NotFound(new { success = false, message = "Cart not found or access denied." });

                return Ok(new { success = true, message = "Cart cleared successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while clearing cart.", error = ex.Message });
            }
        }

        [HttpDelete("remove-product")]
        public async Task<IActionResult> RemoveProductFromCart([FromQuery] int cartItemId, [FromQuery] int productId)
        {
            try
            {
                var currentUserId = GetUserId();
                var role = GetUserRole();

                var success = await work.CartItemRepository.RemoveProductFromCartAsync(cartItemId, productId, currentUserId, role);
                if (!success) return NotFound(new { success = false, message = "Cart or product not found or access denied." });

                return Ok(new { success = true, message = "Product removed from cart successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while removing product from cart.", error = ex.Message });
            }
        }
    }
}