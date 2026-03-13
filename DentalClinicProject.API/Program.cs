using DentalClinicProject.API.Mapping;
using DentalClinicProject.API.Middleware;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Infrastructure;
using DentalClinicProject.Infrastructure.Services;
using FluentValidation.AspNetCore;
using Ganss.Xss;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

namespace DentalClinicProject.API
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Infrastructure
            builder.Services.AddInfrastructure(builder.Configuration);

            // Controllers
            builder.Services.AddControllers();

            // FluentValidation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();

            // OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // AutoMapper
            builder.Services.AddAutoMapper(op =>
            {
                op.AddProfile<UserMapping>();
                op.AddProfile<CartItemMapping>();
                op.AddProfile<AppointmentMapping>();
            });

            // Redis
            builder.Services.AddSingleton<IRedisService, RedisService>();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Cors", policy =>
                {
                    policy.WithOrigins("https://localhost:7114") 
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // JWT Authentication
            var jwtKey = builder.Configuration["JWT:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new InvalidOperationException("JWT:Key is missing");

            var key = Encoding.UTF8.GetBytes(jwtKey);

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Google Auth (optional)
            var googleClientId = builder.Configuration["Google:ClientId"];

            if (!string.IsNullOrWhiteSpace(googleClientId))
            {
                builder.Services
                    .AddAuthentication()
                    .AddGoogle(options =>
                    {
                        options.ClientId = googleClientId;
                        options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
                        options.SaveTokens = true;
                    });
            }

            // Forwarded Headers
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto;
            });

            // Identity Lockout
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            });

            // Rate Limiting
            builder.Services.AddRateLimiter(options =>
            {
                options.AddConcurrencyLimiter("ConcurrencyLimiter", opt =>
                {
                    opt.PermitLimit = 30;
                    opt.QueueLimit = 5;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.AddPolicy("PerIpSliding", context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString();

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        ip ?? "unknown",
                        _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 25,
                            Window = TimeSpan.FromSeconds(10),
                            SegmentsPerWindow = 10,
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("AuthLimiter", context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString();

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        ip ?? "unknown",
                        _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 5,
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });
            });

            builder.Services.AddSingleton<HtmlSanitizer>();

            var app = builder.Build();

            app.UseForwardedHeaders();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Custom Middlewares
            app.UseMiddleware<ScriptInjectionMiddleware>();
            app.UseMiddleware<GlobalMiddlewareException>();
            app.UseMiddleware<CookieToHeaderMiddleware>();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("Cors");

            app.UseAuthentication();

            app.UseMiddleware<BlacklistTokenMiddleware>();

            app.UseAuthorization();

            app.UseRateLimiter();

            app.MapControllers()
               .RequireRateLimiting("PerIpSliding");

            app.Run();
        }
    }
}