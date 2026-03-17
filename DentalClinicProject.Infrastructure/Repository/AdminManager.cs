using AutoMapper;
using DentalClinicProject.Core.DTOs.Auth;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Identity;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class AdminManager : IAdminManager
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public AdminManager(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<AppUser> CreateAccountFromAdminAsync(RegisterDTO dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null || await _userManager.FindByNameAsync(dto.UserName) != null)
            {
                throw new Exception("Email or username already exists.");
            }
            var user = _mapper.Map<AppUser>(dto);

            var result = await _userManager.CreateAsync(user, dto.Password);
            user.EmailConfirmed = true;

            if(string.IsNullOrEmpty(dto.PhoneNumber))
                user.PhoneNumberConfirmed = true;

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, dto.Role.ToString());
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to assign role: {errors}");

            }
            return user;
        }

        public async Task<bool> DeleteAccountFromAdminAsync(DeleteAccountFromAdminDTO dto)
        {
            var user = _userManager.FindByEmailAsync(dto.Email).Result;
            if (user == null)
                throw new Exception("User not found.");

            if(user.Provider == Provider.Google)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to delete user: {errors}");
                }
                return true;
            }

            if(string.IsNullOrEmpty(dto.Password))
                throw new Exception("Password is required for non-External accounts.");

            var check = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (check)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to delete user: {errors}");
                }
                return true;
            }
            else
            {
                throw new Exception("Incorrect password.");
            }
        }

        public async Task<AppUser> UpdateAccountFromAdminAsync(UpdateAccountFromAdminDTO dto)
        {
            var user = _userManager.FindByEmailAsync(dto.Email).Result;
            if (user == null)
                throw new Exception("User not found.");
            var check = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (check)
            {
                user.FirstName = dto.FirstName ?? user.FirstName;
                user.LastName = dto.LastName ?? user.LastName;
                user.UserName = dto.UserName ?? user.UserName;
                user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
                await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                if (dto.Role != null)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, dto.Role.ToString()!);
                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        throw new Exception($"Failed to assign role: {errors}");
                    }
                }
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to update user: {errors}");
                }
                return user;
            }
            else
            {
                throw new Exception("Incorrect password.");
            }
        }
    }
}
