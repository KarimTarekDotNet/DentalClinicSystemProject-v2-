using DentalClinicProject.Core.Entities.AuthModel;
using DentalClinicProject.Core.Entities.Users;
using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Infrastructure.Data.Context;
using DentalClinicProject.Infrastructure.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DentalClinicProject.Infrastructure.Services.AuthHelper
{
    public class TokenService : ITokenService
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;
        private readonly UserManager<AppUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            ApplicationDbContext context, 
            IConfiguration configuration, 
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<TokenService> logger)
        {
            this.context = context;
            this.configuration = configuration;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<string> GenerateAccessToken(AppUser user)
        {
            try
            {
                _logger.LogInformation("Generating access token for user {UserId}", user?.Id);

                // Validation checks
                if (user == null)
                {
                    _logger.LogWarning("GenerateAccessToken failed: User is null");
                    throw new ArgumentNullException(nameof(user), "User cannot be null");
                }

                if (string.IsNullOrWhiteSpace(user.Id))
                {
                    _logger.LogWarning("GenerateAccessToken failed: User ID is empty");
                    throw new ArgumentException("User ID is required", nameof(user));
                }

                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    _logger.LogWarning("GenerateAccessToken failed: User email is empty for user {UserId}", user.Id);
                    throw new ArgumentException("User email is required", nameof(user));
                }

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

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                _logger.LogInformation("Access token generated successfully for user {UserId}", user.Id);
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating access token for user {UserId}", user?.Id);
                throw;
            }
        }

        public string GenerateRefreshToken()
        {
            try
            {
                _logger.LogDebug("Generating refresh token");

                var random = new byte[64];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(random);
                    var token = Convert.ToBase64String(random);
                    _logger.LogDebug("Refresh token generated successfully");
                    return token;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating refresh token");
                throw;
            }
        }

        public RefreshToken? GetRefreshToken(string userId)
        {
            try
            {
                _logger.LogDebug("Getting refresh token for user {UserId}", userId);

                // Validation checks
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("GetRefreshToken failed: UserId is empty");
                    return null;
                }

                var token = context.RefreshTokens
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .AsNoTracking()
                    .AsEnumerable()
                    .FirstOrDefault(r => !r.IsRevoked);

                if (token != null)
                {
                    _logger.LogDebug("Active refresh token found for user {UserId}", userId);
                }
                else
                {
                    _logger.LogDebug("No active refresh token found for user {UserId}", userId);
                }

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting refresh token for user {UserId}", userId);
                return null;
            }
        }

        public async Task RevokeAllUserTokensAsync(string userId, string? ipAddress = null, string? replacedByToken = null)
        {
            try
            {
                _logger.LogInformation("Revoking all tokens for user {UserId}", userId);

                // Validation checks
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("RevokeAllUserTokensAsync failed: UserId is empty");
                    throw new ArgumentException("UserId is required", nameof(userId));
                }

                var tokens = await context.RefreshTokens
                    .Where(r => r.UserId == userId && !r.IsRevoked)
                    .ToListAsync();

                if (!tokens.Any())
                {
                    _logger.LogInformation("No active tokens found to revoke for user {UserId}", userId);
                    return;
                }

                _logger.LogInformation("Revoking {Count} tokens for user {UserId}", tokens.Count, userId);

                foreach (var token in tokens)
                {
                    token.Revoke(ipAddress, replacedByToken);
                }

                await context.SaveChangesAsync();
                _logger.LogInformation("All tokens revoked successfully for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking all tokens for user {UserId}", userId);
                throw;
            }
        }

        public async Task RevokeUserTokensAsync(int tokenId, string userId, string? ipAddress = null, string? replacedByToken = null)
        {
            try
            {
                _logger.LogInformation("Revoking token {TokenId} for user {UserId}", tokenId, userId);

                // Validation checks
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("RevokeUserTokensAsync failed: UserId is empty");
                    throw new ArgumentException("UserId is required", nameof(userId));
                }

                if (tokenId <= 0)
                {
                    _logger.LogWarning("RevokeUserTokensAsync failed: Invalid tokenId {TokenId}", tokenId);
                    throw new ArgumentException("Valid tokenId is required", nameof(tokenId));
                }

                var token = await context.RefreshTokens
                    .FirstOrDefaultAsync(r => r.Id == tokenId && r.UserId == userId);

                if (token is null)
                {
                    _logger.LogWarning("Token {TokenId} not found for user {UserId}", tokenId, userId);
                    return;
                }

                if (!token.IsActive)
                {
                    _logger.LogInformation("Token {TokenId} is already inactive for user {UserId}", tokenId, userId);
                    return;
                }

                token.Revoke(ipAddress, replacedByToken);
                await context.SaveChangesAsync();
                _logger.LogInformation("Token {TokenId} revoked successfully for user {UserId}", tokenId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking token {TokenId} for user {UserId}", tokenId, userId);
                throw;
            }
        }

        public async Task<RefreshToken> SaveRefreshTokenAsync(string userId, string refreshToken)
        {
            try
            {
                _logger.LogInformation("Saving refresh token for user {UserId}", userId);

                // Validation checks
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("SaveRefreshTokenAsync failed: UserId is empty");
                    throw new ArgumentException("UserId is required", nameof(userId));
                }

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    _logger.LogWarning("SaveRefreshTokenAsync failed: RefreshToken is empty for user {UserId}", userId);
                    throw new ArgumentException("RefreshToken is required", nameof(refreshToken));
                }

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
                
                _logger.LogInformation("Refresh token saved successfully for user {UserId}", userId);
                return refreshTokenRecord;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving refresh token for user {UserId}", userId);
                throw;
            }
        }
    }
}