using AutoMapper;
using AutoMapper.QueryableExtensions;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.Entities.Core;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace DentalClinicProject.Infrastructure.Utilities
{
    public static class Helper
    {

        public static IQueryable<Appointment> BaseQuery(ApplicationDbContext _context)
        {
            return _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.AppUser)
                .Include(a => a.Doctor).ThenInclude(d => d.AppUser)
                .Include(a => a.Service)
                .AsNoTracking();
        }

        public static async Task<PagedResult<AppointmentDTO>> BuildAppointmentsQuery(IQueryable<Appointment> query,
            PaginationParams param, ApplicationDbContext _context, IMapper _mapper)
        {
            if (!string.IsNullOrEmpty(param.SearchKeyword))
            {
                var searchTerm = $"%{param.SearchKeyword}%";

                query = query.Where(c =>
                    EF.Functions.Like(c.Patient.AppUser.FirstName, searchTerm) ||
                    EF.Functions.Like(c.Patient.AppUser.LastName, searchTerm) ||
                    EF.Functions.Like(c.Doctor.AppUser.FirstName, searchTerm) ||
                    EF.Functions.Like(c.Doctor.AppUser.LastName, searchTerm) ||
                    EF.Functions.Like(c.Service.Name, searchTerm));
            }

            var totalCount = await query.CountAsync();

            query = param.SortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(c => c.Service.Price),
                "price_desc" => query.OrderByDescending(c => c.Service.Price),
                "created_asc" => query.OrderBy(c => c.CreatedAt),
                "created_desc" => query.OrderByDescending(c => c.CreatedAt),
                _ => query.OrderBy(c => c.Service.Name)
            };

            var items = await query
                .Skip((param.PageNumber - 1) * param.PageSize)
                .Take(param.PageSize)
                .ProjectTo<AppointmentDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<AppointmentDTO>
            {
                Items = items,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                TotalCount = totalCount
            };
        }
        public static string GenerateVerificationCode(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new char[length];

            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[length];
            rng.GetBytes(randomBytes);

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[randomBytes[i] % chars.Length];
            }

            return new string(result);
        }

        public static string GenerateSecureToken(int length = 32)
        {
            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[length];
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public static async Task SendVerificationEmailAsync(string email, IRedisService redisService,
            Core.Interfaces.IServices.IMailService mailService,
            ILogger<AuthService> logger)
        {
            try
            {
                // Normalize email
                email = email.Trim().ToLowerInvariant();
                
                // Get and delete old active code if exists
                var activeCodeKey = RedisKeys.ActiveEmailVerificationCode(email);
                var oldCode = await redisService.GetAsync(activeCodeKey);
                
                if (!string.IsNullOrEmpty(oldCode))
                {
                    // Delete old code
                    var oldCodeKey = RedisKeys.EmailVerificationCode(email, oldCode);
                    await redisService.DeleteAsync(oldCodeKey);
                    logger.LogInformation("Deleted old verification code for email {Email}", email);
                }
                
                // Generate new code (uppercase)
                var code = GenerateVerificationCode().ToUpperInvariant();
                
                // Store new code with email:code pattern (for verification lookup)
                string key = RedisKeys.EmailVerificationCode(email, code);
                logger.LogInformation("Storing verification code - Email: {Email}, Code: {Code}, Key: {Key}", email, code, key);
                
                var result = await redisService.SetAsync(key, code, TimeSpan.FromMinutes(15));
                if (!result)
                {
                    logger.LogError("Failed to store verification code in Redis - Key: {Key}", key);
                    throw new Exception("Failed to store verification code in Redis");
                }
                
                logger.LogInformation("Verification code stored successfully in Redis - Key: {Key}", key);
                
                // Store active code reference (to track and delete old codes)
                await redisService.SetAsync(activeCodeKey, code, TimeSpan.FromMinutes(15));
                
                // Send email
                await mailService.SendVerificationCodeEmail(email, code);
                
                logger.LogInformation("New verification code sent to email {Email}, old code invalidated", email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send verification email to {Email}", email);
                throw;
            }
        }
        public static async Task SendVerificationPhoneAsync(string phone, IRedisService redisService,
            IPhoneService phoneService,
            ILogger<AuthService> logger)
        {
            try
            {
                string key = RedisKeys.PhoneVerificationCode(phone);
                await redisService.SetAsync(key, "PhoneVerify", TimeSpan.FromMinutes(5));
                await phoneService.SendCodeAsync(phone);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send verification phone to {phone}", phone);
                throw;
            }
        }

        public static async Task<bool> CheckExists(string email, string username, UserManager<AppUser> userManager)
        {
            var existingEmail = await userManager.FindByEmailAsync(email);

            var existingUserName = await userManager.FindByNameAsync(username);

            if (existingUserName != null || existingEmail != null)
                return false;

            return true;
        }
    }
}