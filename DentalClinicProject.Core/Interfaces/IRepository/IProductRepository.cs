using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IProductRepository
    {
        Task<ProductDTO?> GetProductWithDetailsAsync(int id);
        Task<IEnumerable<ProductDTO>> GetProductsByIdsAsync(List<int> ids);
        Task<PagedResult<ProductDTO>> GetProductsWithDetailsAsync(PaginationParams param);
        Task<ProductDTO> CreateProductAsync(CreateProductDTO dto);
        Task<ProductDTO> UpdateProductAsync(UpdateProductDTO dto);
        Task<bool> DeleteProductAsync(int id);
    }
}
