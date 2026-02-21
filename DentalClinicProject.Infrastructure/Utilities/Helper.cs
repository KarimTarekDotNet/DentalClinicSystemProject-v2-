using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Core.ViewModels;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Services;
using MailKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace DentalClinicProject.Infrastructure.Utilities
{
    public static class Helper
    {
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

        public static async Task SendVerificationEmailAsync(string email, IRedisService redisService, Core.Interfaces.IServices.IMailService mailService,
            ILogger<AuthService> logger)
        {
            try
            {
                var code = GenerateVerificationCode();
                string key = $"{email}:Code:{code}";
                await redisService.SetAsync(key, code, TimeSpan.FromMinutes(15));
                await mailService.SendVerificationCodeEmail(email, code);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send verification email to {Email}", email);
                throw;
            }
        }

        public static AuthResult Fail(string message) => new AuthResult { Succeeded = false, Message = message };
        public static async Task<bool> CheckExists(string email, string username, UserManager<AppUser> userManager)
        {
            var existingEmail = await userManager.FindByEmailAsync(email);

            var existingUserName = await userManager.FindByNameAsync(username);

            if (existingUserName != null || existingEmail != null)
                return false;

            return true;
        }

        public static AuthResult Fail(IEnumerable<string> errors)
        {
            return new AuthResult
            {
                Succeeded = false,
                Errors = errors.ToList(),
                Message = "Operation failed"
            };
        }

        public static async Task<IdentityResult> AddUserAsync(
            UserManager<AppUser> userManager,
            AppUser user,
            string password,
            string role,
            ApplicationDbContext context)
        {
            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                return createResult;

            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
                return roleResult;

            if (role == "Doctor")
            {
                var doctor = new Doctor
                {
                    AppUserId = user.Id
                };

                await context.Doctors.AddAsync(doctor);
                await context.SaveChangesAsync();
            }

            return IdentityResult.Success;
        }
    }
}