using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IRateRepository
    {
        Task<Rate?> GetRateWithDetailsAsync(int id);
        Task<PagedResult<Rate>> GetRatesWithDetailsAsync(PaginationParams param);
        Task<PagedResult<Rate>> GetDoctorRatesAsync(int doctorId, PaginationParams param);
        Task<PagedResult<Rate>> GetProductRatesAsync(int productId, PaginationParams param);
        Task<Rate?> GetRateByAppointmentAsync(int appointmentId);
        Task<Rate> CreateRateAsync(CreateRateDTO dto);
        Task<bool> UpdateRateAsync(int id, UpdateRateDTO dto);
        Task<bool> DeleteRateAsync(int id);
        Task<double> GetDoctorAverageRatingAsync(int doctorId);
        Task<double> GetProductAverageRatingAsync(int productId);
    }
}
