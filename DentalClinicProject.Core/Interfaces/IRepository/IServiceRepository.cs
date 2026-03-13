using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IServiceRepository
    {
        Task<Service?> GetServiceByIdAsync(int id);
        Task<PagedResult<Service>> GetServicesAsync(PaginationParams param);
        Task<Service> CreateServiceAsync(CreateServiceDTO dto);
        Task<bool> UpdateServiceAsync(UpdateServiceDTO dto);
        Task<bool> DeleteServiceAsync(int id);
        Task<IEnumerable<Service>> GetServicesByIdsAsync(List<int> ids);
        Task<bool> ServiceExistsAsync(int id);
    }
}
