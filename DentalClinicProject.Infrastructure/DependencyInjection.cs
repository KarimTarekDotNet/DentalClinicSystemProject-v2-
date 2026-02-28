using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Core.Validators;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Repository;
using DentalClinicProject.Infrastructure.Services.AuthHelper;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DentalClinicProject.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var connectionString = configuration.GetConnectionString("Redis") ?? throw new Exception("Not Implement");
                return ConnectionMultiplexer.Connect(connectionString);
            });

            services.AddValidatorsFromAssemblyContaining<LoginDTOValidator>();
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                // Password settings - disable built-in validation (we use FluentValidation)
                options.Password.RequiredLength = 1; // Minimum to disable
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;
                
                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IMailService, MailService>();
            services.AddSingleton<IPhoneService, PhoneService>();
            
            return services;
        }
    }
}
