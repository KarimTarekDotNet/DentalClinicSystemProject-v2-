using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class RateRepository : IRateRepository
    {
        private readonly ApplicationDbContext _context;

        public RateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Rate> CreateRateAsync(CreateRateDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRateAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<double> GetDoctorAverageRatingAsync(int doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Rate>> GetDoctorRatesAsync(int doctorId, PaginationParams param)
        {
            throw new NotImplementedException();
        }

        public Task<double> GetProductAverageRatingAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Rate>> GetProductRatesAsync(int productId, PaginationParams param)
        {
            throw new NotImplementedException();
        }

        public Task<Rate?> GetRateByAppointmentAsync(int appointmentId)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Rate>> GetRatesWithDetailsAsync(PaginationParams param)
        {
            throw new NotImplementedException();
        }

        public Task<Rate?> GetRateWithDetailsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRateAsync(UpdateRateDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
