using AutoMapper;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DentalClinicProject.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IMailService _mailService;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UnitOfWork(ApplicationDbContext context, IConfiguration configuration, UserManager<AppUser> userManager,
            IConnectionMultiplexer connectionMultiplexer, IMapper mapper, IHttpContextAccessor httpContextAccessor, IMailService mailService,
            ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _connectionMultiplexer = connectionMultiplexer;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _mailService = mailService;
            _logger = logger;
            TokenService = new TokenService(_context, _configuration, _userManager, httpContextAccessor);
            RedisService = new RedisService(_connectionMultiplexer);
            AuthService = new AuthService(_userManager, _mapper, TokenService, _context, RedisService,
            _mailService, _logger, _httpContextAccessor);
        }

        public ITokenService TokenService { get; }
        public IRedisService RedisService { get; }
        public IAuthService AuthService { get; }
    }
}