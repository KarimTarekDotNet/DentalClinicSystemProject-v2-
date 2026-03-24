using AutoMapper;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Core.Interfaces.Logging;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Services;
using DentalClinicProject.Infrastructure.Services.AuthHelper;
using DentalClinicProject.Infrastructure.Services.SignIn;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UnitOfWork(ApplicationDbContext context, IConfiguration configuration, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IConnectionMultiplexer connectionMultiplexer, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IMailService mailService, IPhoneService phoneService,
            ILogger<AuthService> authLogger, ILogger<EmailVerificationService> emailLogger,
            ILogger<PhoneVerificationService> phoneLogger, ILogger<RedisService> redisLogger, ILogger<TokenService> tokenLogger,
            ILogger<ExternalLoginService> externalLoginLogger, IAppLogger<AppointmentRepository> AppLogger,
            IAppLogger<CartRepository> CartLogger, IAppLogger<ProductRepository> ProductLogger, IAppLogger<ServiceRepository> ServiceLogger,
            IAppLogger<RateRepository> RateLogger)
        {
            _context = context;
            _userManager = userManager;

            RedisService = new RedisService(connectionMultiplexer, redisLogger);
            TokenService = new TokenService(context, configuration, userManager, httpContextAccessor, tokenLogger);
            AuthService = new AuthService(userManager, signInManager, mapper, TokenService, context,
            RedisService, httpContextAccessor, authLogger, mailService, phoneService);
            EmailVerificationService = new EmailVerificationService(userManager, RedisService,
            TokenService, httpContextAccessor, emailLogger, mailService, phoneService, authLogger);
            PhoneVerificationService = new PhoneVerificationService(userManager, phoneService, RedisService, phoneLogger);
            ExternalLoginService = new ExternalLoginService(userManager, signInManager, TokenService,
            RedisService, context, httpContextAccessor, externalLoginLogger);
            AppointmentRepository = new AppointmentRepository(context, userManager, mapper, AppLogger);
            CartRepository = new CartRepository(context, mapper, CartLogger);
            ProductRepository = new ProductRepository(context, mapper, ProductLogger);
            ServiceRepository = new ServiceRepository(context, ServiceLogger);
            RateRepository = new RateRepository(context, mapper, RateLogger);
            AdminManager = new AdminRepository(userManager, mapper);
            ProfileRepository = new ProfileRepository(userManager, mapper, phoneService, RedisService, authLogger);
            OrderRepository = new OrderRepository(context, mapper);
        }

        // Repositories with lazy initialization
        public IAppointmentRepository AppointmentRepository { get; }
        public ICartRepository CartRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IServiceRepository ServiceRepository { get; }
        public IRateRepository RateRepository { get; }
        public IAdminRepository AdminManager { get; }
        public IProfileRepository ProfileRepository { get; }
        public IOrderRepository OrderRepository { get; }

        // Services
        public ITokenService TokenService { get; }
        public IRedisService RedisService { get; }
        public IAuthService AuthService { get; }
        public IEmailVerificationService EmailVerificationService { get; }
        public IPhoneVerificationService PhoneVerificationService { get; }
        public IExternalLoginService ExternalLoginService { get; }
    }
}