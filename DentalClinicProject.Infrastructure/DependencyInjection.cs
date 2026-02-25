using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IRepository;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Core.Validator;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Repository;
using DentalClinicProject.Infrastructure.Services.AuthHelper;
using FluentValidation;
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

            services.AddValidatorsFromAssembly(typeof(ValidRegister).Assembly);
            services.AddValidatorsFromAssemblyContaining<ValidRegister>();

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
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
