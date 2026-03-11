using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Infrastructure.Data.Context;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Product> CreateProductAsync(CreateProductDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetProductsByIdsAsync(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Product>> GetProductsWithDetailsAsync(PaginationParams param)
        {
            throw new NotImplementedException();
        }

        public Task<Product?> GetProductWithDetailsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateProductAsync(int id, UpdateProductDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
