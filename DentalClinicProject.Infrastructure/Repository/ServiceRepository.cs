using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Infrastructure.Data.Context;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<PagedResult<Service>> GetServicesAsync(PaginationParams param)
        {
            throw new NotImplementedException();
        }

        public Task<Service?> GetServiceByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Service>> GetServicesByIdsAsync(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public Task<Service> CreateServiceAsync(CreateServiceDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteServiceAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateServiceAsync(UpdateServiceDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ServiceExistsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}