using AutoMapper;
using AutoMapper.QueryableExtensions;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
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

        public AppointmentRepository(ApplicationDbContext context,
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
            if (dto.ExaminationAppointment.DayOfWeek == DayOfWeek.Friday)
                return false;

            if (dto.ExaminationAppointment <= DateTime.UtcNow)
                return false;

            if (dto.ExaminationAppointment.Hour < 9 || dto.ExaminationAppointment.Hour >= 17)
                return false;

            var doctor = await _context.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == dto.DoctorId);

            if (doctor == null || !doctor.IsApproved)
                return false;

            var appointmentsOnDate = await _context.Appointments
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
                .AnyAsync(s => s.Id == dto.ServiceId);

            if (!serviceExists)
                return false;

            return true;
        }

        public async Task<bool> HasConflictingAppointmentAsync(int doctorId, DateTime appointmentTime, int? excludeAppointmentId = null)
        {
            var start = appointmentTime.AddHours(-1);
            var end = appointmentTime.AddHours(1);

            var query = _context.Appointments
                .Where(a =>
                    a.DoctorId == doctorId &&
                    a.ExaminationEppointment > start &&
                    a.ExaminationEppointment < end);

            if (excludeAppointmentId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAppointmentId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasPatientConflictAsync(int patientId, DateTime appointmentTime, int? excludeAppointmentId = null)
        {
            var start = appointmentTime.AddHours(-1);
            var end = appointmentTime.AddHours(1);

            var query = _context.Appointments
                .Where(a =>
                    a.PatientId == patientId &&
                    a.ExaminationEppointment > start &&
                    a.ExaminationEppointment < end);

            if (excludeAppointmentId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAppointmentId.Value);
            }

            return await query.AnyAsync();
        }

        #endregion

        #region CRUD Operations

        public async Task<AppointmentDTO> CreateAppointmentAsync(CreateAppointmentDTO dto, int patientId)
        {
            if (!await CanBookAppointmentAsync(dto, patientId))
                throw new InvalidOperationException("Cannot book appointment with the provided details.");

            var patient = await _context.Patients
                .Include(p => p.AppUser)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (patient == null)
                throw new InvalidOperationException("Patient not found.");

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == dto.DoctorId);

            if (doctor == null)
                throw new InvalidOperationException("Doctor not found.");

            var serviceExists = await _context.Services
                .AnyAsync(s => s.Id == dto.ServiceId);

            if (!serviceExists)
                throw new InvalidOperationException("Service not found.");

            var user = patient.AppUser;

            if (!await _userManager.IsInRoleAsync(user, "Patient"))
            {
                await _userManager.AddToRoleAsync(user, "Patient");
            }

            var appointment = new Appointment
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                ServiceId = dto.ServiceId,
                ExaminationEppointment = dto.ExaminationAppointment,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            return _mapper.Map<AppointmentDTO>(appointment);
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return false;

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAppointmentAsync(int id, UpdateAppointmentDTO dto)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return false;

            if (dto.DoctorId.HasValue && dto.DoctorId.Value != appointment.DoctorId)
            {
                var doctor = await _context.Doctors
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == dto.DoctorId.Value);

                if (doctor == null || !doctor.IsApproved)
                    return false;

                appointment.DoctorId = dto.DoctorId.Value;
            }

            if (dto.ServiceId.HasValue && dto.ServiceId.Value != appointment.ServiceId)
            {
                var serviceExists = await _context.Services
                    .AnyAsync(s => s.Id == dto.ServiceId.Value);

                if (!serviceExists)
                    return false;

                appointment.ServiceId = dto.ServiceId.Value;
            }

            if (dto.ExaminationAppointment.HasValue &&
                dto.ExaminationAppointment.Value != appointment.ExaminationEppointment)
            {
                var newDate = dto.ExaminationAppointment.Value;

                if (newDate.DayOfWeek == DayOfWeek.Friday)
                    return false;

                if (newDate <= DateTime.UtcNow)
                    return false;

                if (newDate.Hour < 9 || newDate.Hour >= 17)
                    return false;

                if (await HasConflictingAppointmentAsync(appointment.DoctorId, newDate, id))
                    return false;

                if (await HasPatientConflictAsync(appointment.PatientId, newDate, id))
                    return false;

                var doctorCapacity = await _context.Doctors
                    .Where(d => d.Id == appointment.DoctorId)
                    .Select(d => d.CapactiyOfDay)
                    .FirstAsync();

                var appointmentsOnNewDate = await _context.Appointments
                    .CountAsync(a =>
                        a.DoctorId == appointment.DoctorId &&
                        a.ExaminationEppointment.Date == newDate.Date &&
                        a.Id != id);

                if (appointmentsOnNewDate >= doctorCapacity)
                    return false;

                appointment.ExaminationEppointment = newDate;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        #endregion
    }
}