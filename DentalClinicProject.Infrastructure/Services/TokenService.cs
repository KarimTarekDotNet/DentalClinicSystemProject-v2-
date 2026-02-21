using DentalClinicProject.Core.Entities.AuthModel;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DentalClinicProject.Infrastructure.Services
{
    internal class TokenService : ITokenService
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;
        private readonly UserManager<AppUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TokenService(ApplicationDbContext context, IConfiguration configuration, UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.configuration = configuration;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GenerateAccessToken(AppUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("uid", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var random = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        public RefreshToken? GetRefreshToken(string userId)
        {
            return context.RefreshTokens
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking()
                .AsEnumerable()
                .FirstOrDefault(r => !r.IsRevoked);
        }

        public async Task RevokeAllUserTokensAsync(string userId, string? ipAddress = null, string? replacedByToken = null)
        {
            var tokens = await context.RefreshTokens
                .Where(r => r.UserId == userId && !r.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revoke(ipAddress, replacedByToken);
            }

            await context.SaveChangesAsync();
        }

        public async Task RevokeUserTokensAsync(int tokenId, string userId, string? ipAddress = null, string? replacedByToken = null)
        {
            var token = await context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Id == tokenId && r.UserId == userId);

            if (token is not null && token.IsActive)
            {
                token.Revoke(ipAddress, replacedByToken);
                await context.SaveChangesAsync();
            }
        }

        public async Task<RefreshToken> SaveRefreshTokenAsync(string userId, string refreshToken)
        {
            var ipAddress = IpAddressHelper.GetClientIpAddress(httpContextAccessor);
            var refreshTokenRecord = new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(15),
                CreatedByIp = ipAddress
            };

            await context.RefreshTokens.AddAsync(refreshTokenRecord);
            await context.SaveChangesAsync();
            return refreshTokenRecord;
        }
    }
}