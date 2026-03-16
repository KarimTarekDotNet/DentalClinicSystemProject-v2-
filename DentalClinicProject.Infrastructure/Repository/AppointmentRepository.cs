using AutoMapper;
using AutoMapper.QueryableExtensions;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.Logging;
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
        private readonly IAppLogger<AppointmentRepository> _logger;

        private const int WorkingHoursStart = 9;
        private const int WorkingHoursEnd = 17;
        private const int ConflictWindowHours = 1;

        public AppointmentRepository(
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            IMapper mapper,
            IAppLogger<AppointmentRepository> logger)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        #region Get Methods

        public async Task<PagedResult<AppointmentDTO>> GetAppointmentsWithDetailsAsync(PaginationParams param)
        {
            _logger.LogOperationStarted(nameof(GetAppointmentsWithDetailsAsync), new { param.PageNumber, param.PageSize });

            var query = Helper.BaseQuery(_context);
            var result = await Helper.BuildAppointmentsQuery(query, param, _context, _mapper);

            if (!result.Items.Any())
                _logger.LogEmptyResult(nameof(GetAppointmentsWithDetailsAsync));

            _logger.LogOperationCompleted(nameof(GetAppointmentsWithDetailsAsync), new { TotalCount = result.TotalCount });
            return result;
        }

        public async Task<AppointmentDTO?> GetAppointmentWithDetailsAsync(int id)
        {
            _logger.LogOperationStarted(nameof(GetAppointmentWithDetailsAsync), new { AppointmentId = id });

            if (id <= 0)
            {
                _logger.LogValidationError(nameof(GetAppointmentWithDetailsAsync), $"Invalid appointment ID: {id}");
                return null;
            }

            var result = await Helper.BaseQuery(_context)
                .Where(x => x.Id == id)
                .ProjectTo<AppointmentDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (result is null)
                _logger.LogNotFound("Appointment", id);
            else
                _logger.LogOperationCompleted(nameof(GetAppointmentWithDetailsAsync), new { AppointmentId = id });

            return result;
        }

        public async Task<PagedResult<AppointmentDTO>> GetDoctorAppointmentsAsync(int doctorId, PaginationParams param)
        {
            _logger.LogOperationStarted(nameof(GetDoctorAppointmentsAsync), new { DoctorId = doctorId, param.PageNumber, param.PageSize });

            if (doctorId <= 0)
            {
                _logger.LogValidationError(nameof(GetDoctorAppointmentsAsync), $"Invalid doctor ID: {doctorId}");
                return null!;
            }

            var query = Helper.BaseQuery(_context).Where(x => x.DoctorId == doctorId);
            var result = await Helper.BuildAppointmentsQuery(query, param, _context, _mapper);

            if (!result.Items.Any())
                _logger.LogEmptyResult(nameof(GetDoctorAppointmentsAsync), new { DoctorId = doctorId });

            _logger.LogOperationCompleted(nameof(GetDoctorAppointmentsAsync), new { DoctorId = doctorId, TotalCount = result.TotalCount });
            return result;
        }

        public async Task<PagedResult<AppointmentDTO>> GetPatientAppointmentsAsync(int patientId, PaginationParams param)
        {
            _logger.LogOperationStarted(nameof(GetPatientAppointmentsAsync), new { PatientId = patientId, param.PageNumber, param.PageSize });

            if (patientId <= 0)
            {
                _logger.LogValidationError(nameof(GetPatientAppointmentsAsync), $"Invalid patient ID: {patientId}");
                return null!;
            }

            var query = Helper.BaseQuery(_context).Where(x => x.PatientId == patientId);
            var result = await Helper.BuildAppointmentsQuery(query, param, _context, _mapper);

            if (!result.Items.Any())
                _logger.LogEmptyResult(nameof(GetPatientAppointmentsAsync), new { PatientId = patientId });

            _logger.LogOperationCompleted(nameof(GetPatientAppointmentsAsync), new { PatientId = patientId, TotalCount = result.TotalCount });
            return result;
        }

        #endregion

        #region Business Checks

        public async Task<bool> CanBookAppointmentAsync(CreateAppointmentDTO dto, int patientId)
        {
            _logger.LogOperationStarted(nameof(CanBookAppointmentAsync), new { PatientId = patientId, dto.DoctorId, dto.ExaminationAppointment });

            var patientExists = await _context.Patients
                .AsNoTracking()
                .AnyAsync(a => a.Id == patientId);

            if (!patientExists)
            {
                _logger.LogNotFound("Patient", patientId);
                return false;
            }

            if (!IsValidAppointmentTime(dto.ExaminationAppointment))
            {
                _logger.LogBusinessRuleViolation(nameof(CanBookAppointmentAsync),
                    $"Invalid appointment time: {dto.ExaminationAppointment:yyyy-MM-dd HH:mm} — must be within working hours (9–17), not Friday, and in the future.");
                return false;
            }

            var doctor = await _context.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == dto.DoctorId);

            if (doctor is null || !doctor.IsApproved)
            {
                _logger.LogNotFound("Doctor (approved)", dto.DoctorId);
                return false;
            }

            var appointmentsOnDate = await _context.Appointments
                .AsNoTracking()
                .CountAsync(a =>
                    a.DoctorId == dto.DoctorId &&
                    a.ExaminationEppointment.Date == dto.ExaminationAppointment.Date);

            if (appointmentsOnDate >= doctor.CapactiyOfDay)
            {
                _logger.LogBusinessRuleViolation(nameof(CanBookAppointmentAsync),
                    $"Doctor {dto.DoctorId} has reached daily capacity ({doctor.CapactiyOfDay}) on {dto.ExaminationAppointment.Date:yyyy-MM-dd}.");
                return false;
            }

            if (await HasConflictingAppointmentAsync(dto.DoctorId, dto.ExaminationAppointment))
            {
                _logger.LogBusinessRuleViolation(nameof(CanBookAppointmentAsync),
                    $"Doctor {dto.DoctorId} has a conflicting appointment within {ConflictWindowHours}h of {dto.ExaminationAppointment:HH:mm}.");
                return false;
            }

            if (await HasPatientConflictAsync(patientId, dto.ExaminationAppointment))
            {
                _logger.LogBusinessRuleViolation(nameof(CanBookAppointmentAsync),
                    $"Patient {patientId} has a conflicting appointment within {ConflictWindowHours}h of {dto.ExaminationAppointment:HH:mm}.");
                return false;
            }

            var serviceExists = await _context.Services
                .AsNoTracking()
                .AnyAsync(s => s.Id == dto.ServiceId);

            if (!serviceExists)
            {
                _logger.LogNotFound("Service", dto.ServiceId);
                return false;
            }

            return true;
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
            _logger.LogOperationStarted(nameof(CreateAppointmentAsync), new { dto.PatientId, dto.DoctorId, dto.ServiceId, dto.ExaminationAppointment });

            if (!await CanBookAppointmentAsync(dto, dto.PatientId))
            {
                _logger.LogBusinessRuleViolation(nameof(CreateAppointmentAsync),
                    $"Booking rejected for PatientId={dto.PatientId}, DoctorId={dto.DoctorId}, Date={dto.ExaminationAppointment:yyyy-MM-dd HH:mm}.");
                throw new InvalidOperationException("Unable to book the appointment. Please verify the doctor availability, working hours (9 AM–5 PM), and that the selected date is not a Friday.");
            }

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

            _logger.LogOperationCompleted(nameof(CreateAppointmentAsync), new { AppointmentId = appointment.Id, dto.PatientId, dto.DoctorId });

            var createdAppointment = await GetAppointmentWithIncludesAsync(appointment.Id);
            return _mapper.Map<AppointmentDTO>(createdAppointment);
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            _logger.LogOperationStarted(nameof(CancelAppointmentAsync), new { AppointmentId = id });

            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);

            if (appointment is null)
            {
                _logger.LogNotFound("Appointment", id);
                return false;
            }

            if (appointment.ExaminationEppointment <= DateTime.UtcNow)
            {
                _logger.LogBusinessRuleViolation(nameof(CancelAppointmentAsync),
                    $"Cannot cancel past appointment {id} scheduled at {appointment.ExaminationEppointment:yyyy-MM-dd HH:mm}.");
                return false;
            }

            if (appointment.ExaminationEppointment - DateTime.UtcNow <= TimeSpan.FromHours(24))
            {
                _logger.LogBusinessRuleViolation(nameof(CancelAppointmentAsync),
                    $"Cannot cancel appointment {id} — less than 24 hours remaining before scheduled time.");
                return false;
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(CancelAppointmentAsync), new { AppointmentId = id });
            return true;
        }

        public async Task<ApiResponse<AppointmentDTO>> UpdateAppointmentAsync(UpdateAppointmentDTO dto)
        {
            _logger.LogOperationStarted(nameof(UpdateAppointmentAsync), new { dto.Id });

            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == dto.Id);

            if (appointment is null)
            {
                _logger.LogNotFound("Appointment", dto.Id);
                return Fail("No appointment found with the provided ID.");
            }

            var newDoctorId = dto.DoctorId ?? appointment.DoctorId;
            var newServiceId = dto.ServiceId ?? appointment.ServiceId;
            var newDate = dto.ExaminationAppointment ?? appointment.ExaminationEppointment;

            if (newDoctorId != appointment.DoctorId)
            {
                var doctor = await _context.Doctors
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == newDoctorId);

                if (doctor is null)
                {
                    _logger.LogNotFound("Doctor", newDoctorId);
                    return Fail($"No doctor found with ID {newDoctorId}.");
                }

                if (!doctor.IsApproved)
                {
                    _logger.LogBusinessRuleViolation(nameof(UpdateAppointmentAsync), $"Doctor {newDoctorId} is not approved.");
                    return Fail("The selected doctor has not been approved yet.");
                }
            }

            if (newServiceId != appointment.ServiceId)
            {
                var serviceExists = await _context.Services.AnyAsync(s => s.Id == newServiceId);
                if (!serviceExists)
                {
                    _logger.LogNotFound("Service", newServiceId);
                    return Fail($"No service found with ID {newServiceId}.");
                }
            }

            if (newDate != appointment.ExaminationEppointment)
            {
                var dateValidationError = await ValidateAppointmentDateAsync(newDoctorId, newDate, appointment.PatientId, dto.Id);
                if (dateValidationError is not null)
                {
                    _logger.LogBusinessRuleViolation(nameof(UpdateAppointmentAsync), dateValidationError);
                    return Fail(dateValidationError);
                }
            }

            appointment.DoctorId = newDoctorId;
            appointment.ServiceId = newServiceId;
            appointment.ExaminationEppointment = newDate;

            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(UpdateAppointmentAsync), new { AppointmentId = appointment.Id });

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
            if (appointmentTime.DayOfWeek == DayOfWeek.Friday) return false;
            if (appointmentTime <= DateTime.UtcNow) return false;
            if (appointmentTime.Hour < WorkingHoursStart || appointmentTime.Hour >= WorkingHoursEnd) return false;
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