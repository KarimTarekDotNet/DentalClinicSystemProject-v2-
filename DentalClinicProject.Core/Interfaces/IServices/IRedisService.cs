namespace DentalClinicProject.Core.Interfaces.IServices
{
    public interface IRedisService
    {
        Task<string?> GetAsync(string Key);
        Task<bool> SetAsync(string Key, string value, TimeSpan expiry);
        Task<bool> DeleteAsync(string key);
        Task<bool> BlacklistTokenAsync(string token, TimeSpan expiry);
        Task<bool> IsTokenBlacklistedAsync(string token);
    }
}
