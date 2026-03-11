namespace DentalClinicProject.Core.Interfaces.IServices
{
    public interface IRedisService
    {
        Task<string?> GetAsync(string Key);
        Task<bool> SetAsync(string Key, string value, TimeSpan expiry);
        Task<bool> DeleteAsync(string key);
        Task<bool> BlacklistTokenAsync(string token, TimeSpan expiry);
        Task<bool> IsTokenBlacklistedAsync(string token);
        
        // Generic caching methods
        Task<T?> GetObjectAsync<T>(string key) where T : class;
        Task<bool> SetObjectAsync<T>(string key, T value, TimeSpan expiry) where T : class;
        Task<bool> DeleteByPatternAsync(string pattern);
    }
}
