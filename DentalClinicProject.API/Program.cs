using DentalClinicProject.API.Mapping;
using DentalClinicProject.API.Middleware;
using DentalClinicProject.Core.Validators;
using DentalClinicProject.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
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

            builder.Services.AddInfrastructure(builder.Configuration);

            // Add FluentValidation
            builder.Services.AddControllers();

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();

            builder.Services.AddOpenApi();

            builder.Services.AddAutoMapper(op =>
            {
                op.AddProfile<UserMapping>();
            });

            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(op =>
            {
                op.AddPolicy("Cors", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyOrigin()
                          .AllowAnyMethod();
                });
            });

            // Authentication
            var jwtKey = builder.Configuration["JWT:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException("JWT:Key is not configured in appsettings or user secrets");
            }
            
            var key = Encoding.UTF8.GetBytes(jwtKey);

            builder.Services.AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(op =>
            {
                op.RequireHttpsMetadata = true;
                op.SaveToken = false;

                op.TokenValidationParameters = new TokenValidationParameters
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
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto;
            });
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            });

            builder.Services.AddRateLimiter(option =>
            {
                option.AddConcurrencyLimiter("ConcurrencyLimiter", opt =>
                {
                    opt.PermitLimit = 30;
                    opt.QueueLimit = 5;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                }).AddPolicy("PerIpSliding", context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString();

                    return RateLimitPartition.GetSlidingWindowLimiter(ip ?? "Unknown", _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 25,
                        Window = TimeSpan.FromSeconds(10),
                        SegmentsPerWindow = 10,
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    });
                }).AddPolicy("AuthLimiter", context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString();

                    return RateLimitPartition.GetSlidingWindowLimiter(ip ?? "Unknown", _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 5,
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
                });
            });

            var app = builder.Build();
            app.UseForwardedHeaders();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapOpenApi();
            }
            app.UseMiddleware<GlobalMiddlewareException>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("Cors");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers().RequireRateLimiting("PerIpSliding");

            app.Run();
        }
    }
}