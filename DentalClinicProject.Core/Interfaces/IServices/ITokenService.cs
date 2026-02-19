using DentalClinicProject.Core.Entities.AuthModel;
using DentalClinicProject.Core.Entities.Users;

namespace DentalClinicProject.Core.Interfaces.IServices
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(AppUser user);
        string GenerateRefreshToken();
        Task<RefreshToken> SaveRefreshTokenAsync(string userId, string refreshToken);
        RefreshToken? GetRefreshToken(string userId);
        Task RevokeAllUserTokensAsync(string userId, string? ipAddress = null, string? replacedByToken = null);
        Task RevokeUserTokensAsync(int tokenId, string userId, string? ipAddress = null, string? replacedByToken = null);
    }
}