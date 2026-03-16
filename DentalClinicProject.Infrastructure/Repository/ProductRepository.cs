using AutoMapper;
using AutoMapper.QueryableExtensions;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.Logging;
using DentalClinicProject.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAppLogger<ProductRepository> _logger;

        public ProductRepository(ApplicationDbContext context, IMapper mapper, IAppLogger<ProductRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByIdsAsync(List<int> ids)
        {
            _logger.LogOperationStarted(nameof(GetProductsByIdsAsync), new { Ids = ids });

            var products = await _context.Products
                .AsNoTracking()
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            if (products is null || !products.Any())
            {
                _logger.LogEmptyResult(nameof(GetProductsByIdsAsync), new { RequestedIds = ids });
                return Enumerable.Empty<ProductDTO>();
            }

            _logger.LogOperationCompleted(nameof(GetProductsByIdsAsync), new { Found = products.Count });
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<PagedResult<ProductDTO>> GetProductsWithDetailsAsync(PaginationParams param)
        {
            _logger.LogOperationStarted(nameof(GetProductsWithDetailsAsync), new { param.PageNumber, param.PageSize, param.SearchKeyword });

            var query = _context.Products.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(param.SearchKeyword))
            {
                var searchTerm = $"%{param.SearchKeyword}%";
                query = query.Where(c =>
                    EF.Functions.Like(c.Name, searchTerm) ||
                    EF.Functions.Like(c.Description, searchTerm));
            }

            var totalCount = await query.CountAsync();

            query = param.SortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(c => c.Price),
                "price_desc" => query.OrderByDescending(c => c.Price),
                "created_asc" => query.OrderBy(c => c.CreatedAt),
                "created_desc" => query.OrderByDescending(c => c.CreatedAt),
                _ => query.OrderBy(c => c.Name)
            };

            var items = await query
                .Skip((param.PageNumber - 1) * param.PageSize)
                .Take(param.PageSize)
                .ProjectTo<ProductDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!items.Any())
                _logger.LogEmptyResult(nameof(GetProductsWithDetailsAsync), new { param.SearchKeyword });

            _logger.LogOperationCompleted(nameof(GetProductsWithDetailsAsync), new { TotalCount = totalCount, Returned = items.Count });

            return new PagedResult<ProductDTO>
            {
                Items = items,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ProductDTO?> GetProductWithDetailsAsync(int id)
        {
            _logger.LogOperationStarted(nameof(GetProductWithDetailsAsync), new { ProductId = id });

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null)
            {
                _logger.LogNotFound(nameof(Product), id);
                return null;
            }

            _logger.LogOperationCompleted(nameof(GetProductWithDetailsAsync), new { ProductId = id });
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> CreateProductAsync(CreateProductDTO dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            _logger.LogOperationStarted(nameof(CreateProductAsync), new { dto.Name, dto.Price });

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(CreateProductAsync), new { ProductId = product.Id, product.Name });
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            _logger.LogOperationStarted(nameof(DeleteProductAsync), new { ProductId = id });

            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                _logger.LogNotFound(nameof(Product), id);
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(DeleteProductAsync), new { ProductId = id });
            return true;
        }

        public async Task<ProductDTO> UpdateProductAsync(UpdateProductDTO dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            _logger.LogOperationStarted(nameof(UpdateProductAsync), new { dto.Id });

            var product = await _context.Products.FindAsync(dto.Id);
            if (product is null)
            {
                _logger.LogNotFound(nameof(Product), dto.Id);
                throw new KeyNotFoundException($"No product found with ID {dto.Id}.");
            }

            if (string.IsNullOrEmpty(dto.Name)) dto.Name = product.Name;
            if (string.IsNullOrEmpty(dto.Description)) dto.Description = product.Description;
            if (dto.Price is null) dto.Price = product.Price;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price.Value;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(UpdateProductAsync), new { ProductId = product.Id });
            return _mapper.Map<ProductDTO>(product);
        }
    }
}