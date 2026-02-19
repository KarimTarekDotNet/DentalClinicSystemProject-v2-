using DentalClinicProject.Core.Interfaces.IServices;
using StackExchange.Redis;

namespace DentalClinicProject.Infrastructure.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisService(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<bool> SetAsync(string Key, string value, TimeSpan expiry)
        {
            return await _database.StringSetAsync(Key, value, expiry);
        }

        public async Task<bool> DeleteAsync(string key)
        {
            return await _database.KeyDeleteAsync(key);
        }

        public async Task<string?> GetAsync(string Key)
        {
            return await _database.StringGetAsync(Key);
        }

        public async Task<bool> BlacklistTokenAsync(string token, TimeSpan expiry)
        {
            var key = $"blacklist:{token}";
            return await _database.StringSetAsync(key, "revoked", expiry);
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            var key = $"blacklist:{token}";
            return await _database.KeyExistsAsync(key);
        }
    }
}