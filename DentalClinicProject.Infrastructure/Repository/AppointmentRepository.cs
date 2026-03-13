using AutoMapper;
using AutoMapper.QueryableExtensions;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.ViewModels;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        private const int WorkingHoursStart = 9;
        private const int WorkingHoursEnd = 17;
        private const int ConflictWindowHours = 1;

        public AppointmentRepository(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        #region Get Methods

        public async Task<PagedResult<AppointmentDTO>> GetAppointmentsWithDetailsAsync(PaginationParams param)
        {
            var query = Helper.BaseQuery(_context);
            return await Helper.BuildAppointmentsQuery(query, param, _context, _mapper);
        }

        public async Task<AppointmentDTO?> GetAppointmentWithDetailsAsync(int id)
        {
            if (id <= 0)
                return null;

            return await Helper.BaseQuery(_context)
                .Where(x => x.Id == id)
                .ProjectTo<AppointmentDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<AppointmentDTO>> GetDoctorAppointmentsAsync(int doctorId, PaginationParams param)
        {
            if (doctorId <= 0)
                return null!;

            var query = Helper.BaseQuery(_context)
                .Where(x => x.DoctorId == doctorId);

            return await Helper.BuildAppointmentsQuery(query, param, _context, _mapper);
        }

        public async Task<PagedResult<AppointmentDTO>> GetPatientAppointmentsAsync(int patientId, PaginationParams param)
        {
            if (patientId <= 0)
                return null!;

            var query = Helper.BaseQuery(_context)
                .Where(x => x.PatientId == patientId);

            return await Helper.BuildAppointmentsQuery(query, param, _context, _mapper);
        }

        #endregion

        #region Business Checks

        public async Task<bool> CanBookAppointmentAsync(CreateAppointmentDTO dto, int patientId)
        {
            var patientExists = await _context.Patients
                .AsNoTracking()
                .AnyAsync(a => a.Id == patientId);

            if (!patientExists)
                return false;

            if (!IsValidAppointmentTime(dto.ExaminationAppointment))
                return false;

            var doctor = await _context.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == dto.DoctorId);

            if (doctor == null || !doctor.IsApproved)
                return false;

            var appointmentsOnDate = await _context.Appointments
                .AsNoTracking()
                .CountAsync(a =>
                    a.DoctorId == dto.DoctorId &&
                    a.ExaminationEppointment.Date == dto.ExaminationAppointment.Date);

            if (appointmentsOnDate >= doctor.CapactiyOfDay)
                return false;

            if (await HasConflictingAppointmentAsync(dto.DoctorId, dto.ExaminationAppointment))
                return false;

            if (await HasPatientConflictAsync(patientId, dto.ExaminationAppointment))
                return false;

            var serviceExists = await _context.Services
                .AsNoTracking()
                .AnyAsync(s => s.Id == dto.ServiceId);

            return serviceExists;
        }

        public async Task<bool> HasConflictingAppointmentAsync(int doctorId, DateTime appointmentTime, int? excludeAppointmentId = null)
        {
            var start = appointmentTime.AddHours(-ConflictWindowHours);
            var end = appointmentTime.AddHours(ConflictWindowHours);

            var query = _context.Appointments
                .Where(a =>
                    a.DoctorId == doctorId &&
                    a.ExaminationEppointment > start &&
                    a.ExaminationEppointment < end);

            if (excludeAppointmentId.HasValue)
                query = query.Where(a => a.Id != excludeAppointmentId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> HasPatientConflictAsync(int patientId, DateTime appointmentTime, int? excludeAppointmentId = null)
        {
            var start = appointmentTime.AddHours(-ConflictWindowHours);
            var end = appointmentTime.AddHours(ConflictWindowHours);

            var query = _context.Appointments
                .Where(a =>
                    a.PatientId == patientId &&
                    a.ExaminationEppointment > start &&
                    a.ExaminationEppointment < end);

            if (excludeAppointmentId.HasValue)
                query = query.Where(a => a.Id != excludeAppointmentId.Value);

            return await query.AnyAsync();
        }

        #endregion

        #region CRUD Operations

        public async Task<AppointmentDTO> CreateAppointmentAsync(CreateAppointmentDTO dto)
        {
            if (!await CanBookAppointmentAsync(dto, dto.PatientId))
                throw new InvalidOperationException("Unable to book the appointment. Please verify the doctor availability, working hours (9 AM–5 PM), and that the selected date is not a Friday.");

            var patient = await _context.Patients
                .Include(p => p.AppUser)
                .FirstOrDefaultAsync(p => p.Id == dto.PatientId)
                ?? throw new InvalidOperationException($"No patient found with ID {dto.PatientId}.");

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == dto.DoctorId)
                ?? throw new InvalidOperationException($"No doctor found with ID {dto.DoctorId}.");

            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == dto.ServiceId)
                ?? throw new InvalidOperationException($"No service found with ID {dto.ServiceId}.");

            if (!await _userManager.IsInRoleAsync(patient.AppUser, "Patient"))
                await _userManager.AddToRoleAsync(patient.AppUser, "Patient");

            var appointment = new Appointment
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                ServiceId = service.Id,
                ExaminationEppointment = dto.ExaminationAppointment,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            var createdAppointment = await GetAppointmentWithIncludesAsync(appointment.Id);
            return _mapper.Map<AppointmentDTO>(createdAppointment);
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return false;

            if (appointment.ExaminationEppointment <= DateTime.UtcNow)
                return false;

            if (appointment.ExaminationEppointment - DateTime.UtcNow <= TimeSpan.FromHours(24))
                return false;

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ApiResponse<AppointmentDTO>> UpdateAppointmentAsync(UpdateAppointmentDTO dto)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == dto.Id);

            if (appointment == null)
                return Fail("No appointment found with the provided ID.");

            var newDoctorId = dto.DoctorId ?? appointment.DoctorId;
            var newServiceId = dto.ServiceId ?? appointment.ServiceId;
            var newDate = dto.ExaminationAppointment ?? appointment.ExaminationEppointment;

            if (newDoctorId != appointment.DoctorId)
            {
                var doctor = await _context.Doctors
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == newDoctorId);

                if (doctor == null)
                    return Fail($"No doctor found with ID {newDoctorId}.");

                if (!doctor.IsApproved)
                    return Fail("The selected doctor has not been approved yet.");
            }

            if (newServiceId != appointment.ServiceId)
            {
                var serviceExists = await _context.Services
                    .AnyAsync(s => s.Id == newServiceId);

                if (!serviceExists)
                    return Fail($"No service found with ID {newServiceId}.");
            }

            if (newDate != appointment.ExaminationEppointment)
            {
                var dateValidationError = await 
                    ValidateAppointmentDateAsync(newDoctorId, newDate, appointment.PatientId, dto.Id);
                if (dateValidationError != null)
                    return Fail(dateValidationError);
            }

            appointment.DoctorId = newDoctorId;
            appointment.ServiceId = newServiceId;
            appointment.ExaminationEppointment = newDate;

            await _context.SaveChangesAsync();

            var updatedAppointment = await GetAppointmentWithIncludesAsync(appointment.Id);

            return new ApiResponse<AppointmentDTO>
            {
                Success = true,
                Message = "Appointment updated successfully.",
                Data = _mapper.Map<AppointmentDTO>(updatedAppointment)
            };
        }

        #endregion

        #region Private Helpers

        private static bool IsValidAppointmentTime(DateTime appointmentTime)
        {
            if (appointmentTime.DayOfWeek == DayOfWeek.Friday)
                return false;

            if (appointmentTime <= DateTime.UtcNow)
                return false;

            if (appointmentTime.Hour < WorkingHoursStart || appointmentTime.Hour >= WorkingHoursEnd)
                return false;

            return true;
        }

        private async Task<string?> ValidateAppointmentDateAsync(int doctorId, DateTime newDate, int patientId, int excludeAppointmentId)
        {
            if (newDate.DayOfWeek == DayOfWeek.Friday)
                return "Appointments cannot be scheduled on Fridays.";

            if (newDate <= DateTime.UtcNow)
                return "Appointment date must be set in the future.";

            if (newDate.Hour < WorkingHoursStart || newDate.Hour >= WorkingHoursEnd)
                return $"Appointments are only available between {WorkingHoursStart}:00 AM and {WorkingHoursEnd - 12}:00 PM.";

            if (await HasConflictingAppointmentAsync(doctorId, newDate, excludeAppointmentId))
                return "The doctor already has another appointment within an hour of this time.";

            if (await HasPatientConflictAsync(patientId, newDate, excludeAppointmentId))
                return "The patient already has another appointment within an hour of this time.";

            var doctorCapacity = await _context.Doctors
                .Where(d => d.Id == doctorId)
                .Select(d => d.CapactiyOfDay)
                .FirstAsync();

            var appointmentsOnDate = await _context.Appointments
                .CountAsync(a =>
                    a.DoctorId == doctorId &&
                    a.ExaminationEppointment.Date == newDate.Date &&
                    a.Id != excludeAppointmentId);

            if (appointmentsOnDate >= doctorCapacity)
                return "The doctor has reached the maximum appointment capacity for this day.";

            return null;
        }

        private async Task<Appointment?> GetAppointmentWithIncludesAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.AppUser)
                .Include(a => a.Patient).ThenInclude(p => p.AppUser)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        private static ApiResponse<AppointmentDTO> Fail(string message) =>
            new() { Success = false, Message = message };

        #endregion
    }
}