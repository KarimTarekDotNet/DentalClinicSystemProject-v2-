using AutoMapper;
using AutoMapper.QueryableExtensions;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public CartItemRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<CartItemDTO>> GetCartItemsWithProductsAsync(string currentUserId, Role role, int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var query = _context.CartItems
                .Include(ci => ci.Products)
                .AsQueryable();

            if (role != Role.Admin)
                query = query.Where(ci => ci.UserId == currentUserId);

            var projected = query.ProjectTo<CartItemDTO>(_mapper.ConfigurationProvider).AsNoTracking();
            var totalCount = await projected.CountAsync();

            var items = await projected
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CartItemDTO>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<CartItemDTO?> GetCartItemWithProductsAsync(int id, string currentUserId, Role role)
        {
            var query = _context.CartItems
                .Include(ci => ci.Products)
                .Where(ci => ci.Id == id);

            if (role != Role.Admin)
                query = query.Where(ci => ci.UserId == currentUserId);

            var cartItemDto = await query
                .ProjectTo<CartItemDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return cartItemDto;
        }

        public async Task<CartItemDTO> CreateCartItemAsync(CreateCartItemDTO dto, string currentUserId)
        {
            var products = await _context.Products
                .Where(p => dto.ProductIds.Contains(p.Id))
                .ToListAsync();

            if (!products.Any())
                throw new Exception("No valid products to create cart.");

            var cartItem = new CartItem
            {
                UserId = currentUserId,
                Products = products,
                ItemCount = products.Count,
                TotalPrice = products.Sum(p => p.Price)
            };

            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();

            return _mapper.Map<CartItemDTO>(cartItem);
        }

        private async Task<CartItem?> GetCartForModificationAsync(int cartItemId, string currentUserId, Role role)
        {
            var query = _context.CartItems
                .Include(ci => ci.Products)
                .Where(ci => ci.Id == cartItemId);

            if (role != Role.Admin)
                query = query.Where(ci => ci.UserId == currentUserId);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> AddProductToCartAsync(int cartItemId, int productId, string currentUserId, Role role)
        {
            var cartItem = await GetCartForModificationAsync(cartItemId, currentUserId, role);
            if (cartItem == null) return false;

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return false;

            if (!cartItem.Products.Any(p => p.Id == productId))
            {
                cartItem.Products.Add(product);
                cartItem.ItemCount += 1;
                cartItem.TotalPrice += product.Price;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveProductFromCartAsync(int cartItemId, int productId, string currentUserId, Role role)
        {
            var cartItem = await GetCartForModificationAsync(cartItemId, currentUserId, role);
            if (cartItem == null) return false;

            var product = cartItem.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null) return false;

            cartItem.Products.Remove(product);
            cartItem.ItemCount -= 1;
            cartItem.TotalPrice -= product.Price;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(int cartItemId, string currentUserId, Role role)
        {
            var cartItem = await GetCartForModificationAsync(cartItemId, currentUserId, role);
            if (cartItem == null) return false;

            cartItem.Products.Clear();
            cartItem.ItemCount = 0;
            cartItem.TotalPrice = 0;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CartItemDTO> UpdateCartItemAsync(UpdateCartItemDTO dto, string currentUserId, Role role)
        {
            var cartItem = await GetCartForModificationAsync(dto.Id, currentUserId, role);
            if (cartItem == null)
                throw new Exception("Cart item not found or access denied.");

            if (dto.ProductIds != null && dto.ProductIds.Any())
            {
                var products = await _context.Products
                    .Where(p => dto.ProductIds.Contains(p.Id))
                    .ToListAsync();

                cartItem.Products.Clear();
                foreach (var p in products)
                    cartItem.Products.Add(p);
                cartItem.ItemCount = products.Count;
                cartItem.TotalPrice = products.Sum(p => p.Price);
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<CartItemDTO>(cartItem);
        }
    }
}