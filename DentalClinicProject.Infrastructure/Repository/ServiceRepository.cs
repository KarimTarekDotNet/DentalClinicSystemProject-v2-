using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.Logging;
using DentalClinicProject.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppLogger<ServiceRepository> _logger;

        public ServiceRepository(ApplicationDbContext context, IAppLogger<ServiceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<Service>> GetServicesAsync(PaginationParams param)
        {
            _logger.LogOperationStarted(nameof(GetServicesAsync), new { param.PageNumber, param.PageSize, param.SearchKeyword });

            var query = _context.Services.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(param.SearchKeyword))
                query = query.Where(s => s.Name.Contains(param.SearchKeyword));

            if (!string.IsNullOrEmpty(param.SortBy))
            {
                query = param.SortBy!.ToLower() switch
                {
                    "name" => query.OrderBy(s => s.Name),
                    "price" => query.OrderBy(s => s.Price),
                    "duration" => query.OrderBy(s => s.DurationInMinutes),
                    _ => query.OrderBy(s => s.Id)
                };
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((param.PageNumber - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToListAsync();

            if (!items.Any())
                _logger.LogEmptyResult(nameof(GetServicesAsync), new { param.SearchKeyword });

            _logger.LogOperationCompleted(nameof(GetServicesAsync), new { TotalCount = totalCount, Returned = items.Count });

            return new PagedResult<Service>
            {
                Items = items,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            _logger.LogOperationStarted(nameof(GetServiceByIdAsync), new { ServiceId = id });

            var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);

            if (service is null)
            {
                _logger.LogNotFound(nameof(Service), id);
                return null;
            }

            _logger.LogOperationCompleted(nameof(GetServiceByIdAsync), new { ServiceId = id });
            return service;
        }

        public async Task<IEnumerable<Service>> GetServicesByIdsAsync(List<int> ids)
        {
            _logger.LogOperationStarted(nameof(GetServicesByIdsAsync), new { Ids = ids });

            var services = await _context.Services
                .Where(s => ids.Contains(s.Id))
                .AsNoTracking()
                .ToListAsync();

            if (!services.Any())
            {
                _logger.LogEmptyResult(nameof(GetServicesByIdsAsync), new { RequestedIds = ids });
                return null!;
            }

            _logger.LogOperationCompleted(nameof(GetServicesByIdsAsync), new { Found = services.Count });
            return services;
        }

        public async Task<Service> CreateServiceAsync(CreateServiceDTO dto)
        {
            _logger.LogOperationStarted(nameof(CreateServiceAsync), new { dto.Name, dto.Price });

            var service = new Service
            {
                Name = dto.Name,
                Price = dto.Price,
                DurationInMinutes = dto.DurationInMinutes
            };

            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(CreateServiceAsync), new { ServiceId = service.Id, service.Name });
            return service;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            _logger.LogOperationStarted(nameof(DeleteServiceAsync), new { ServiceId = id });

            var service = await _context.Services.FindAsync(id);
            if (service is null)
            {
                _logger.LogNotFound(nameof(Service), id);
                return false;
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(DeleteServiceAsync), new { ServiceId = id });
            return true;
        }

        public async Task<Service> UpdateServiceAsync(UpdateServiceDTO dto)
        {
            _logger.LogOperationStarted(nameof(UpdateServiceAsync), new { dto.Id });

            var service = await _context.Services.FindAsync(dto.Id);
            if (service is null)
            {
                _logger.LogNotFound(nameof(Service), dto.Id);
                throw new KeyNotFoundException("No service found with this id.");
            }

            if (string.IsNullOrEmpty(dto.Name)) dto.Name = service.Name;
            if (dto.Price is null) dto.Price = service.Price;
            if (dto.DurationInMinutes is null) dto.DurationInMinutes = service.DurationInMinutes;

            service.Name = dto.Name;
            service.Price = dto.Price.Value;
            service.DurationInMinutes = dto.DurationInMinutes.Value;

            _context.Services.Update(service);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(UpdateServiceAsync), new { ServiceId = service.Id });
            return service;
        }

        public async Task<bool> ServiceExistsAsync(int id)
        {
            return await _context.Services.AnyAsync(s => s.Id == id);
        }
    }
}