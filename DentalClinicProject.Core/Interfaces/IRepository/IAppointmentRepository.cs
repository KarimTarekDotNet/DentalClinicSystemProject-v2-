using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.ViewModels;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IAppointmentRepository
    {
        Task<AppointmentDTO?> GetAppointmentWithDetailsAsync(int id);
        Task<PagedResult<AppointmentDTO>> GetAppointmentsWithDetailsAsync(PaginationParams param);
        Task<PagedResult<AppointmentDTO>> GetDoctorAppointmentsAsync(int doctorId, PaginationParams param);
        Task<PagedResult<AppointmentDTO>> GetPatientAppointmentsAsync(int patientId, PaginationParams param);
        Task<bool> CanBookAppointmentAsync(CreateAppointmentDTO dto, int patientId);
        Task<AppointmentDTO> CreateAppointmentAsync(CreateAppointmentDTO dto);
        Task<ApiResponse<AppointmentDTO>> UpdateAppointmentAsync(UpdateAppointmentDTO dto);
        Task<bool> CancelAppointmentAsync(int id);
        Task<bool> HasConflictingAppointmentAsync(int doctorId, DateTime appointmentTime, int? excludeAppointmentId = null);
        Task<bool> HasPatientConflictAsync(int patientId, DateTime appointmentTime, int? excludeAppointmentId = null);
    }
}