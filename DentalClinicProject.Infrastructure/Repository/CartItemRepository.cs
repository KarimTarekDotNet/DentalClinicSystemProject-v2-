using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.Logging;
using DentalClinicProject.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAppLogger<CartRepository> _logger;

        public CartRepository(ApplicationDbContext context, IMapper mapper, IAppLogger<CartRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CartDTO?> GetCartByUserIdAsync(string userId, Role role)
        {
            _logger.LogOperationStarted(nameof(GetCartByUserIdAsync), new { UserId = userId, Role = role });

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null)
            {
                _logger.LogNotFound("Cart", $"UserId={userId}");
                return null;
            }

            _logger.LogOperationCompleted(nameof(GetCartByUserIdAsync), new { UserId = userId, ItemsCount = cart.Items.Count });
            return _mapper.Map<CartDTO>(cart);
        }

        public async Task<bool> AddProductToCartAsync(int productId, string userId, Role role, int quantity = 1)
        {
            _logger.LogOperationStarted(nameof(AddProductToCartAsync), new { ProductId = productId, UserId = userId, Quantity = quantity });

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null)
            {
                cart = new Cart { UserId = userId };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
                _logger.LogOperationCompleted("CreateCart", new { UserId = userId, CartId = cart.Id });
            }

            var product = await _context.Products.FindAsync(productId);
            if (product is null)
            {
                _logger.LogNotFound("Product", productId);
                return false;
            }

            var existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existing is not null)
            {
                existing.Quantity += quantity;
                _logger.LogOperationCompleted("UpdateCartItemQuantity", new { ProductId = productId, NewQuantity = existing.Quantity });
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                });
                _logger.LogOperationCompleted("AddNewCartItem", new { ProductId = productId, Quantity = quantity });
            }

            await _context.SaveChangesAsync();
            _logger.LogOperationCompleted(nameof(AddProductToCartAsync), new { ProductId = productId, UserId = userId });
            return true;
        }

        public async Task<bool> RemoveProductFromCartAsync(int cartId, int productId, string userId, Role role, int quantity = 1)
        {
            _logger.LogOperationStarted(nameof(RemoveProductFromCartAsync), new { CartId = cartId, ProductId = productId, UserId = userId, Quantity = quantity });

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart is null)
            {
                _logger.LogNotFound("Cart", cartId);
                return false;
            }

            if (role != Role.Admin && cart.UserId != userId)
            {
                _logger.LogUnauthorizedAccess(nameof(RemoveProductFromCartAsync), userId);
                return false;
            }

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null)
            {
                _logger.LogNotFound("CartItem", $"ProductId={productId} in CartId={cartId}");
                return false;
            }

            if (item.Quantity < quantity)
            {
                _logger.LogBusinessRuleViolation(nameof(RemoveProductFromCartAsync),
                    $"Requested removal quantity ({quantity}) exceeds current item quantity ({item.Quantity}) for ProductId={productId}.");
                return false;
            }

            item.Quantity -= quantity;

            if (item.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
                _logger.LogOperationCompleted("RemoveCartItem", new { ProductId = productId, CartId = cartId });
            }
            else
            {
                _logger.LogOperationCompleted("DecreaseCartItemQuantity", new { ProductId = productId, RemainingQuantity = item.Quantity });
            }

            await _context.SaveChangesAsync();
            _logger.LogOperationCompleted(nameof(RemoveProductFromCartAsync), new { CartId = cartId, ProductId = productId });
            return true;
        }

        public async Task<bool> ClearCartAsync(int cartId, string userId, Role role)
        {
            _logger.LogOperationStarted(nameof(ClearCartAsync), new { CartId = cartId, UserId = userId });

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart is null)
            {
                _logger.LogNotFound("Cart", cartId);
                return false;
            }

            if (role != Role.Admin && cart.UserId != userId)
            {
                _logger.LogUnauthorizedAccess(nameof(ClearCartAsync), userId);
                return false;
            }

            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(ClearCartAsync), new { CartId = cartId, RemovedItems = cart.Items.Count });
            return true;
        }
    }
}