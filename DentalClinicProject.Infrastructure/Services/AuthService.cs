using AutoMapper;
using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Core.ViewModels;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DentalClinicProject.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMailService mailService;
        private readonly ILogger<AuthService> logger;
        private readonly UserManager<AppUser> userManager;
        private readonly ApplicationDbContext context;
        private readonly ITokenService tokenService;
        private readonly IRedisService redisService;
        private readonly IMapper mapper;
        public AuthService(UserManager<AppUser> userManager, IMapper mapper, ITokenService tokenService, ApplicationDbContext context,
            IRedisService redisService, IMailService mailService, ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.tokenService = tokenService;
            this.context = context;
            this.redisService = redisService;
            this.mailService = mailService;
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
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
            await Helper.SendVerificationEmailAsync(registerDTO.Email!, redisService, mailService, logger);

            var result = new AuthResult
            {
                Email = user.Email,
                Role = registerDTO.Role.ToString(),
                Succeeded = true,
                Username = user.UserName,
                UserId = user.Id,
                Message = "Account created successfully"
            };

            return new ApiResponse<AuthResult>
            {
                Success = true,
                StatusCode = 200,
                Message = "Account created successfully Please Verify Your Email",
                Data = result
            };
        }

        public async Task<ApiResponse<AuthResult>> VerifyEmail(string email, string code)
        {
            try
            {
                string key = $"{email}:Code:{code}";
                var savedCode = await redisService.GetAsync(key);
                if (savedCode == null)
                {
                    logger.LogWarning("Invalid verification code for {Email}", email);
                    return new ApiResponse<AuthResult>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Invalid or expired verification code"
                    };
                }

                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ApiResponse<AuthResult>
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "User not found"
                    };
                }

                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);
                await redisService.DeleteAsync(key);
                var ipAddress = IpAddressHelper.GetClientIpAddress(httpContextAccessor);
                await tokenService.RevokeAllUserTokensAsync(user.Id, ipAddress, "EmailVerification");

                var accessToken = await tokenService.GenerateAccessToken(user);
                var refreshToken = tokenService.GenerateRefreshToken();
                var refresh = await tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

                string Refreshkey = $"{user.Id}:User:{refresh.CreatedByIp}";
                await redisService.SetAsync(Refreshkey, refreshToken, TimeSpan.FromDays(15));

                var roles = await userManager.GetRolesAsync(user);

                logger.LogInformation("Email verified for {Email}", email);
                var result = new AuthResult
                {
                    Succeeded = true,
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Role = roles.FirstOrDefault(),
                    Message = "Email verified successfully"
                };
                return new ApiResponse<AuthResult>
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Account created successfully Please Login Again",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during email verification for {Email}", email);
                return new ApiResponse<AuthResult>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "An error occurred during email verification"
                };
            }
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