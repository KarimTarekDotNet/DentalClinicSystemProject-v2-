using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Helpers;
using System.Threading.Tasks;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IRateRepository
    {
        // Doctor Rates
        Task<AvargeDoctorRateDTO?> GetDoctorAverageRatingAsync(int doctorId);
        Task<PagedResult<RateDoctorDTO>> GetDoctorRatesAsync(int doctorId, int pageNumber = 1, int pageSize = 10);
        Task<RateDoctorDTO?> GetDoctorRateAsync(int id);
        Task<RateDoctorDTO> CreateDoctorRateAsync(string userId, CreateDoctorRateDTO dto);
        Task<RateDoctorDTO> UpdateDoctorRateAsync(string userId, UpdateRateDTO dto);
        Task<bool> DeleteDoctorRateAsync(string userId, int id);

        // Product Rates
        Task<AvargeProductRateDTO?> GetProductAverageRatingAsync(int productId);
        Task<PagedResult<RateProductDTO>> GetProductRatesAsync(int productId, int pageNumber = 1, int pageSize = 10);
        Task<RateProductDTO?> GetProductRateAsync(int id);
        Task<RateProductDTO> CreateProductRateAsync(string userId, CreateProductRateDTO dto);
        Task<RateProductDTO> UpdateProductRateAsync(string userId, UpdateRateDTO dto);
        Task<bool> DeleteProductRateAsync(string userId, int id);

        // Appointment Rates
        Task<RateAppointmentDTO?> GetRateByAppointmentAsync(int appointmentId);
        Task<RateAppointmentDTO?> GetAppointmentRateAsync(int id);
        Task<RateAppointmentDTO> CreateAppointmentRateAsync(string userId, CreateApponitmentRateDTO dto);
        Task<RateAppointmentDTO> UpdateAppointmentRateAsync(string userId, UpdateRateDTO dto);
        Task<bool> DeleteAppointmentRateAsync(string userId, int id);

        // Generic
        Task<PagedResult<RateDTO>> GetRatesWithDetailsAsync(int pageNumber = 1, int pageSize = 10);
        Task<RateDTO?> GetRateWithDetailsAsync(int id);
    }
}