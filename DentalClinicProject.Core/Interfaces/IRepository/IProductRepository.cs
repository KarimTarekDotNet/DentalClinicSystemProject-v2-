using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IProductRepository
    {
        Task<Product?> GetProductWithDetailsAsync(int id);
        Task<PagedResult<Product>> GetProductsWithDetailsAsync(PaginationParams param);
        Task<Product> CreateProductAsync(CreateProductDTO dto);
        Task<bool> UpdateProductAsync(int id, UpdateProductDTO dto);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsByIdsAsync(List<int> ids);
    }
}
