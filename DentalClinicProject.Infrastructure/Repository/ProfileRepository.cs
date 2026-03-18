using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhoneService _phoneService;
        private readonly IRedisService _redisService;
        private ILogger<AuthService> logger;
        private readonly IMapper _mapper;

        public ProfileRepository(UserManager<AppUser> userManager, IMapper mapper,
            IPhoneService phoneService, IRedisService redisService, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _phoneService = phoneService;
            _redisService = redisService;
            this.logger = logger;
        }

        public async Task<bool> DeleteProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            await _userManager.DeleteAsync(user);
            return true;
        }

        public async Task<AppUserDTO?> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;
            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<AppUserDTO>(user);
            userDto.Role = roles.FirstOrDefault() ?? "User";
            return userDto;
        }

        public async Task<IdentityResult> UpdateProfileAsync(string userId, UpdateAccountDTO user)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);
            if (existingUser == null)
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found"
                });

            if (!string.IsNullOrWhiteSpace(user.FirstName))
                existingUser.FirstName = user.FirstName;

            if (!string.IsNullOrWhiteSpace(user.LastName))
                existingUser.LastName = user.LastName;

            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                var userWithSameUsername = await _userManager.FindByNameAsync(user.UserName);
                if (userWithSameUsername != null && userWithSameUsername.Id != userId)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "Username already taken"
                    });
                }

                existingUser.UserName = user.UserName;
            }

            bool phoneChanged = false;

            if (!string.IsNullOrWhiteSpace(user.PhoneNumber) &&
                user.PhoneNumber != existingUser.PhoneNumber)
            {
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.PhoneNumberConfirmed = false;
                phoneChanged = true;
            }

            var result = await _userManager.UpdateAsync(existingUser);

            if (result.Succeeded && phoneChanged)
                await Helper.SendVerificationPhoneAsync(existingUser.PhoneNumber!, _redisService, _phoneService, logger);

            return result;
        }
    }
}