using AutoMapper;
using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Core.ViewModels;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;

namespace DentalClinicProject.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ApplicationDbContext context;
        private readonly ITokenService tokenService;
        private readonly IRedisService redisService;
        private readonly IMapper mapper;
        public AuthService(UserManager<AppUser> userManager, IMapper mapper, ITokenService tokenService, ApplicationDbContext context, IRedisService redisService)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.tokenService = tokenService;
            this.context = context;
            this.redisService = redisService;
        }

        public async Task<ApiResponse<AuthResult>> Register(RegisterDTO registerDTO)
        {
            var user = mapper.Map<AppUser>(registerDTO);

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var checkEmailAndUsername =
                    await Helper.CheckExists(registerDTO.Email, registerDTO.UserName, userManager);

                if (!checkEmailAndUsername)
                {
                    return new ApiResponse<AuthResult>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Email or username already exists"
                    };
                }

                var addUser = await Helper.AddUserAsync(
                    userManager,
                    user,
                    registerDTO.Password,
                    registerDTO.Role.ToString(),
                    context
                );

                if (!addUser.Succeeded)
                {
                    return new ApiResponse<AuthResult>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "User creation failed",
                        Errors = addUser.Errors.Select(x => x.Description)
                    };
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            var accessToken = await tokenService.GenerateAccessToken(user);
            var refreshToken = tokenService.GenerateRefreshToken();
            var refresh = await tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

            string key = $"{user.Id}:User:{refresh.CreatedByIp}";
            await redisService.SetAsync(key, refreshToken, TimeSpan.FromDays(15));

            var result = new AuthResult
            {
                Email = user.Email,
                RefreshToken = refreshToken,
                Role = registerDTO.Role.ToString(),
                Succeeded = true,
                Token = accessToken,
                Username = user.UserName,
                UserId = user.Id,
                Message = "Account created successfully"
            };

            return new ApiResponse<AuthResult>
            {
                Success = true,
                StatusCode = 200,
                Message = "Account created successfully",
                Data = result
            };
        }


        public Task<AuthResult> Login(LoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogoutAllAsync(string userId, string accessToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogoutAsync(string userId, string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}