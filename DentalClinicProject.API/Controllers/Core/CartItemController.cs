using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DentalClinicProject.API.Controllers.Core
{
    [Authorize]
    [Route("api/cart")]
    public class CartController : BaseController
    {
        public CartController(IUnitOfWork work) : base(work) { }

        private string GetUserId() => User.FindFirst("uid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        private Role GetUserRole() => Enum.Parse<Role>(User.FindFirstValue(ClaimTypes.Role)!);

        // GET api/cart/get
        [HttpGet("get")]
        public async Task<IActionResult> GetUserCart()
        {
            try
            {
                var cart = await work.CartRepository.GetCartByUserIdAsync(GetUserId(), GetUserRole());

                if (cart is null)
                    return NotFound(new { success = false, message = "Cart not found." });

                return Ok(new { success = true, data = cart });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving cart.",
                    error = ex.Message
                });
            }
        }

        // POST api/cart/add-product
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProductToCart([FromQuery] int productId, [FromQuery] int quantity = 1)
        {
            try
            {
                var success = await work.CartRepository.AddProductToCartAsync(productId, GetUserId(), GetUserRole(), quantity);

                if (!success)
                    return NotFound(new
                    {
                        success = false,
                        message = "Cart or product not found or access denied."
                    });

                return Ok(new { success = true, message = "Product added to cart successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while adding product to cart.",
                    error = ex.Message
                });
            }
        }

        // DELETE api/cart/remove-product
        [HttpDelete("remove-product")]
        public async Task<IActionResult> RemoveProductFromCart([FromQuery] int cartId, [FromQuery] int productId, [FromQuery] int quantity = 1)
        {
            try
            {
                var success = await work.CartRepository.RemoveProductFromCartAsync(
                    cartId, productId, GetUserId(), GetUserRole(), quantity);

                if (!success)
                    return NotFound(new
                    {
                        success = false,
                        message = "Cart or product not found or access denied."
                    });

                return Ok(new { success = true, message = "Product removed from cart successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while removing product from cart.",
                    error = ex.Message
                });
            }
        }

        // DELETE api/cart/clear
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart([FromQuery] int cartId)
        {
            try
            {
                var success = await work.CartRepository.ClearCartAsync(
                    cartId, GetUserId(), GetUserRole());

                if (!success)
                    return NotFound(new
                    {
                        success = false,
                        message = "Cart not found or access denied."
                    });

                return Ok(new { success = true, message = "Cart cleared successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while clearing cart.",
                    error = ex.Message
                });
            }
        }
    }
}