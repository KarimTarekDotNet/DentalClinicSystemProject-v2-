using AutoMapper;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Services;
using DentalClinicProject.Infrastructure.Services.AuthHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(ApplicationDbContext context, IConfiguration configuration, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IConnectionMultiplexer connectionMultiplexer, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IMailService mailService, IPhoneService phoneService,
            ILogger<UnitOfWork> logger, ILogger<AuthService> authLogger, ILogger<EmailVerificationService> emailLogger,
            ILogger<PhoneVerificationService> phoneLogger, ILogger<RedisService> redisLogger, ILogger<TokenService> tokenLogger)
        {
            _context = context;
            _logger = logger;
            RedisService = new RedisService(connectionMultiplexer, redisLogger);
            TokenService = new TokenService(context, configuration, userManager, httpContextAccessor, tokenLogger);
            AuthService = new AuthService(userManager, signInManager, mapper, TokenService, context,
            RedisService, httpContextAccessor, authLogger, mailService, phoneService);
            EmailVerificationService = new EmailVerificationService(userManager, RedisService,
            TokenService, httpContextAccessor, emailLogger);
            PhoneVerificationService = new PhoneVerificationService(userManager, phoneService, RedisService, phoneLogger);
        }

        public ITokenService TokenService { get; }
        public IRedisService RedisService { get; }
        public IAuthService AuthService { get; }
        public IEmailVerificationService EmailVerificationService { get; }
        public IPhoneVerificationService PhoneVerificationService { get; }
    }
}