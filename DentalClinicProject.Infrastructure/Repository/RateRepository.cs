using AutoMapper;
using AutoMapper.QueryableExtensions;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.Logging;
using DentalClinicProject.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class RateRepository : IRateRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAppLogger<RateRepository> _logger;

        public RateRepository(ApplicationDbContext context, IMapper mapper, IAppLogger<RateRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ─── Generic ────────────────────────────────────────────────────────────

        public async Task<PagedResult<RateDTO>> GetRatesWithDetailsAsync(int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            _logger.LogOperationStarted(nameof(GetRatesWithDetailsAsync), new { pageNumber, pageSize });

            var query = _context.Rates
                .AsNoTracking()
                .ProjectTo<RateDTO>(_mapper.ConfigurationProvider);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!items.Any())
                _logger.LogEmptyResult(nameof(GetRatesWithDetailsAsync));

            _logger.LogOperationCompleted(nameof(GetRatesWithDetailsAsync), new { TotalCount = totalCount, Returned = items.Count });

            return new PagedResult<RateDTO>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<RateDTO?> GetRateWithDetailsAsync(int id)
        {
            _logger.LogOperationStarted(nameof(GetRateWithDetailsAsync), new { RateId = id });

            var rate = await _context.Rates
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rate is null)
            {
                _logger.LogNotFound("Rate", id);
                return null;
            }

            _logger.LogOperationCompleted(nameof(GetRateWithDetailsAsync), new { RateId = id });
            return _mapper.Map<RateDTO>(rate);
        }

        // ─── Doctor Rates ────────────────────────────────────────────────────────

        public async Task<AvargeDoctorRateDTO?> GetDoctorAverageRatingAsync(int doctorId)
        {
            _logger.LogOperationStarted(nameof(GetDoctorAverageRatingAsync), new { DoctorId = doctorId });

            var doctor = await _context.Doctors
                .Include(d => d.AppUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == doctorId && d.IsApproved);

            if (doctor is null)
            {
                _logger.LogNotFound("Doctor (approved)", doctorId);
                return null;
            }

            var rates = await _context.DoctorRates
                .Where(r => r.DoctorId == doctorId)
                .AsNoTracking()
                .ToListAsync();

            var average = rates.Any()
                ? Math.Round(rates.Average(r => (double)r.Value), 2)
                : 0.0;

            _logger.LogOperationCompleted(nameof(GetDoctorAverageRatingAsync), new { DoctorId = doctorId, Average = average, RatesCount = rates.Count });

            return new AvargeDoctorRateDTO
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.AppUser.FirstName + " " + doctor.AppUser.LastName,
                AverageRate = average,
                Value = rates.Any() ? average.ToString("F1") : "0",
                Comments = rates.Select(r => r.Comment).ToList()
            };
        }

        public async Task<PagedResult<RateDoctorDTO>> GetDoctorRatesAsync(int doctorId, int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            _logger.LogOperationStarted(nameof(GetDoctorRatesAsync), new { DoctorId = doctorId, pageNumber, pageSize });

            var query = _context.DoctorRates
                .Where(r => r.DoctorId == doctorId && r.Doctor.IsApproved)
                .AsNoTracking()
                .ProjectTo<RateDoctorDTO>(_mapper.ConfigurationProvider);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!items.Any())
                _logger.LogEmptyResult(nameof(GetDoctorRatesAsync), new { DoctorId = doctorId });

            _logger.LogOperationCompleted(nameof(GetDoctorRatesAsync), new { DoctorId = doctorId, TotalCount = totalCount });

            return new PagedResult<RateDoctorDTO>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<RateDoctorDTO?> GetDoctorRateAsync(int id)
        {
            _logger.LogOperationStarted(nameof(GetDoctorRateAsync), new { DoctorRateId = id });

            var result = await _context.DoctorRates
                .AsNoTracking()
                .ProjectTo<RateDoctorDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result is null)
                _logger.LogNotFound("DoctorRate", id);
            else
                _logger.LogOperationCompleted(nameof(GetDoctorRateAsync), new { DoctorRateId = id });

            return result;
        }

        public async Task<RateDoctorDTO> CreateDoctorRateAsync(string userId, CreateDoctorRateDTO dto)
        {
            _logger.LogOperationStarted(nameof(CreateDoctorRateAsync), new { UserId = userId, dto.DoctorId, dto.Value });

            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                _logger.LogNotFound("User", userId);
                throw new KeyNotFoundException("User not found.");
            }

            var rate = new DoctorRate
            {
                UserId = userId,
                DoctorId = dto.DoctorId,
                Value = dto.Value,
                Comment = dto.Comment
            };

            await _context.DoctorRates.AddAsync(rate);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(CreateDoctorRateAsync), new { DoctorRateId = rate.Id, UserId = userId });

            return await _context.DoctorRates
                .AsNoTracking()
                .ProjectTo<RateDoctorDTO>(_mapper.ConfigurationProvider)
                .FirstAsync(r => r.Id == rate.Id);
        }

        public async Task<RateDoctorDTO> UpdateDoctorRateAsync(string userId, UpdateRateDTO dto)
        {
            _logger.LogOperationStarted(nameof(UpdateDoctorRateAsync), new { UserId = userId, DoctorRateId = dto.Id });

            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                _logger.LogNotFound("User", userId);
                throw new KeyNotFoundException("User not found.");
            }

            var rate = await _context.DoctorRates.FindAsync(dto.Id);
            if (rate is null)
            {
                _logger.LogNotFound("DoctorRate", dto.Id);
                throw new KeyNotFoundException($"No doctor rate found with ID {dto.Id}.");
            }

            if (string.IsNullOrEmpty(dto.Comment))
                dto.Comment = rate.Comment;

            rate.Value = dto.Value!.Value;
            rate.Comment = dto.Comment;

            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(UpdateDoctorRateAsync), new { DoctorRateId = rate.Id });

            return await _context.DoctorRates
                .AsNoTracking()
                .ProjectTo<RateDoctorDTO>(_mapper.ConfigurationProvider)
                .FirstAsync(r => r.Id == rate.Id);
        }

        public async Task<bool> DeleteDoctorRateAsync(string userId, int id)
        {
            _logger.LogOperationStarted(nameof(DeleteDoctorRateAsync), new { UserId = userId, DoctorRateId = id });

            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                _logger.LogNotFound("User", userId);
                throw new KeyNotFoundException("User not found.");
            }

            var rate = await _context.DoctorRates.FindAsync(id);
            if (rate is null)
            {
                _logger.LogNotFound("DoctorRate", id);
                return false;
            }

            _context.DoctorRates.Remove(rate);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(DeleteDoctorRateAsync), new { DoctorRateId = id });
            return true;
        }

        // ─── Product Rates ───────────────────────────────────────────────────────

        public async Task<AvargeProductRateDTO?> GetProductAverageRatingAsync(int productId)
        {
            _logger.LogOperationStarted(nameof(GetProductAverageRatingAsync), new { ProductId = productId });

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product is null)
            {
                _logger.LogNotFound("Product", productId);
                return null;
            }

            var rates = await _context.ProductRates
                .Where(r => r.ProductId == productId)
                .AsNoTracking()
                .ToListAsync();

            var average = rates.Any()
                ? Math.Round(rates.Average(r => (double)r.Value), 2)
                : 0.0;

            _logger.LogOperationCompleted(nameof(GetProductAverageRatingAsync), new { ProductId = productId, Average = average, RatesCount = rates.Count });

            return new AvargeProductRateDTO
            {
                ProductId = product.Id,
                ProductName = product.Name,
                AverageRate = average,
                Value = rates.Any() ? average.ToString("F1") : "0",
                Comments = rates.Select(r => r.Comment).ToList()
            };
        }

        public async Task<PagedResult<RateProductDTO>> GetProductRatesAsync(int productId, int pageNumber = 1, int pageSize = 10)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            _logger.LogOperationStarted(nameof(GetProductRatesAsync), new { ProductId = productId, pageNumber, pageSize });

            var query = _context.ProductRates
                .Where(r => r.ProductId == productId)
                .AsNoTracking()
                .ProjectTo<RateProductDTO>(_mapper.ConfigurationProvider);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!items.Any())
                _logger.LogEmptyResult(nameof(GetProductRatesAsync), new { ProductId = productId });

            _logger.LogOperationCompleted(nameof(GetProductRatesAsync), new { ProductId = productId, TotalCount = totalCount });

            return new PagedResult<RateProductDTO>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<RateProductDTO?> GetProductRateAsync(int id)
        {
            _logger.LogOperationStarted(nameof(GetProductRateAsync), new { ProductRateId = id });

            var result = await _context.ProductRates
                .AsNoTracking()
                .ProjectTo<RateProductDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result is null)
                _logger.LogNotFound("ProductRate", id);
            else
                _logger.LogOperationCompleted(nameof(GetProductRateAsync), new { ProductRateId = id });

            return result;
        }

        public async Task<RateProductDTO> CreateProductRateAsync(string userId, CreateProductRateDTO dto)
        {
            _logger.LogOperationStarted(nameof(CreateProductRateAsync), new { UserId = userId, dto.ProductId, dto.Value });

            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                _logger.LogNotFound("User", userId);
                throw new KeyNotFoundException("User not found.");
            }

            var rate = new ProductRate
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Value = dto.Value,
                Comment = dto.Comment
            };

            await _context.ProductRates.AddAsync(rate);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(CreateProductRateAsync), new { ProductRateId = rate.Id, UserId = userId });

            return await _context.ProductRates
                .AsNoTracking()
                .ProjectTo<RateProductDTO>(_mapper.ConfigurationProvider)
                .FirstAsync(r => r.Id == rate.Id);
        }

        public async Task<RateProductDTO> UpdateProductRateAsync(string userId, UpdateRateDTO dto)
        {
            _logger.LogOperationStarted(nameof(UpdateProductRateAsync), new { UserId = userId, ProductRateId = dto.Id });

            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                _logger.LogNotFound("User", userId);
                throw new KeyNotFoundException("User not found.");
            }

            var rate = await _context.ProductRates.FindAsync(dto.Id);
            if (rate is null)
            {
                _logger.LogNotFound("ProductRate", dto.Id);
                throw new KeyNotFoundException($"No product rate found with ID {dto.Id}.");
            }

            if (string.IsNullOrEmpty(dto.Comment))
                dto.Comment = rate.Comment;

            rate.Value = dto.Value!.Value;
            rate.Comment = dto.Comment;

            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(UpdateProductRateAsync), new { ProductRateId = rate.Id });

            return await _context.ProductRates
                .AsNoTracking()
                .ProjectTo<RateProductDTO>(_mapper.ConfigurationProvider)
                .FirstAsync(r => r.Id == rate.Id);
        }

        public async Task<bool> DeleteProductRateAsync(string userId, int id)
        {
            _logger.LogOperationStarted(nameof(DeleteProductRateAsync), new { UserId = userId, ProductRateId = id });

            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                _logger.LogNotFound("User", userId);
                throw new KeyNotFoundException("User not found.");
            }

            var rate = await _context.ProductRates.FindAsync(id);
            if (rate is null)
            {
                _logger.LogNotFound("ProductRate", id);
                return false;
            }

            _context.ProductRates.Remove(rate);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(DeleteProductRateAsync), new { ProductRateId = id });
            return true;
        }

        // ─── Appointment Rates ───────────────────────────────────────────────────

        public async Task<RateAppointmentDTO?> GetRateByAppointmentAsync(int appointmentId)
        {
            _logger.LogOperationStarted(nameof(GetRateByAppointmentAsync), new { AppointmentId = appointmentId });

            var result = await _context.AppointmentRates
                .Where(r => r.AppointmentId == appointmentId)
                .AsNoTracking()
                .ProjectTo<RateAppointmentDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (result is null)
                _logger.LogNotFound("AppointmentRate", $"AppointmentId={appointmentId}");
            else
                _logger.LogOperationCompleted(nameof(GetRateByAppointmentAsync), new { AppointmentId = appointmentId });

            return result;
        }

        public async Task<RateAppointmentDTO?> GetAppointmentRateAsync(int id)
        {
            _logger.LogOperationStarted(nameof(GetAppointmentRateAsync), new { AppointmentRateId = id });

            var result = await _context.AppointmentRates
                .AsNoTracking()
                .ProjectTo<RateAppointmentDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result is null)
                _logger.LogNotFound("AppointmentRate", id);
            else
                _logger.LogOperationCompleted(nameof(GetAppointmentRateAsync), new { AppointmentRateId = id });

            return result;
        }

        public async Task<RateAppointmentDTO> CreateAppointmentRateAsync(string userId, CreateApponitmentRateDTO dto)
        {
            _logger.LogOperationStarted(nameof(CreateAppointmentRateAsync), new { UserId = userId, dto.AppointmentId, dto.Value });

            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                _logger.LogNotFound("User", userId);
                throw new KeyNotFoundException("User not found.");
            }

            var rate = new AppointmentRate
            {
                UserId = userId,
                AppointmentId = dto.AppointmentId,
                Value = dto.Value,
                Comment = dto.Comment
            };

            await _context.AppointmentRates.AddAsync(rate);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(CreateAppointmentRateAsync), new { AppointmentRateId = rate.Id, UserId = userId });

            return await _context.AppointmentRates
                .AsNoTracking()
                .ProjectTo<RateAppointmentDTO>(_mapper.ConfigurationProvider)
                .FirstAsync(r => r.Id == rate.Id);
        }

        public async Task<RateAppointmentDTO> UpdateAppointmentRateAsync(string userId, UpdateRateDTO dto)
        {
            _logger.LogOperationStarted(nameof(UpdateAppointmentRateAsync), new { UserId = userId, AppointmentRateId = dto.Id });

            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                _logger.LogNotFound("User", userId);
                throw new KeyNotFoundException("User not found.");
            }

            var rate = await _context.AppointmentRates.FindAsync(dto.Id);
            if (rate is null)
            {
                _logger.LogNotFound("AppointmentRate", dto.Id);
                throw new KeyNotFoundException($"No appointment rate found with ID {dto.Id}.");
            }

            if (string.IsNullOrEmpty(dto.Comment))
                dto.Comment = rate.Comment;

            rate.Value = dto.Value!.Value;
            rate.Comment = dto.Comment;

            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(UpdateAppointmentRateAsync), new { AppointmentRateId = rate.Id });

            return await _context.AppointmentRates
                .AsNoTracking()
                .ProjectTo<RateAppointmentDTO>(_mapper.ConfigurationProvider)
                .FirstAsync(r => r.Id == rate.Id);
        }

        public async Task<bool> DeleteAppointmentRateAsync(string userId, int id)
        {
            _logger.LogOperationStarted(nameof(DeleteAppointmentRateAsync), new { UserId = userId, AppointmentRateId = id });

            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                _logger.LogNotFound("User", userId);
                throw new KeyNotFoundException("User not found.");
            }

            var rate = await _context.AppointmentRates.FindAsync(id);
            if (rate is null)
            {
                _logger.LogNotFound("AppointmentRate", id);
                return false;
            }

            _context.AppointmentRates.Remove(rate);
            await _context.SaveChangesAsync();

            _logger.LogOperationCompleted(nameof(DeleteAppointmentRateAsync), new { AppointmentRateId = id });
            return true;
        }
    }
}