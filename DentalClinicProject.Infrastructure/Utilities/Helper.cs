using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Core.ViewModels;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DentalClinicProject.Infrastructure.Utilities
{
    public static class Helper
    {
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