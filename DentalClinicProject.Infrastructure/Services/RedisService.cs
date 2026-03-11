using DentalClinicProject.Core.Interfaces.IServices;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace DentalClinicProject.Infrastructure.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<RedisService> _logger;

        public RedisService(
            IConnectionMultiplexer connectionMultiplexer,
            ILogger<RedisService> logger)
        {
            _database = connectionMultiplexer.GetDatabase();
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
        }

        public async Task<bool> SetAsync(string Key, string value, TimeSpan expiry)
        {
            try
            {
                // Validation checks
                if (string.IsNullOrWhiteSpace(Key))
                {
                    _logger.LogWarning("Redis SetAsync failed: Key is empty");
                    return false;
                }

                if (value == null)
                {
                    _logger.LogWarning("Redis SetAsync failed: Value is null for key {Key}", Key);
                    return false;
                }

                if (expiry <= TimeSpan.Zero)
                {
                    _logger.LogWarning("Redis SetAsync failed: Invalid expiry time for key {Key}", Key);
                    return false;
                }

                var result = await _database.StringSetAsync(Key, value, expiry);
                
                if (result)
                {
                    _logger.LogDebug("Redis key {Key} set successfully with expiry {Expiry}", Key, expiry);
                }
                else
                {
                    _logger.LogWarning("Redis SetAsync failed for key {Key}", Key);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting Redis key {Key}", Key);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                // Validation checks
                if (string.IsNullOrWhiteSpace(key))
                {
                    _logger.LogWarning("Redis DeleteAsync failed: Key is empty");
                    return false;
                }

                var result = await _database.KeyDeleteAsync(key);
                
                if (result)
                {
                    _logger.LogDebug("Redis key {Key} deleted successfully", key);
                }
                else
                {
                    _logger.LogDebug("Redis key {Key} not found for deletion", key);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Redis key {Key}", key);
                return false;
            }
        }

        public async Task<string?> GetAsync(string Key)
        {
            try
            {
                // Validation checks
                if (string.IsNullOrWhiteSpace(Key))
                {
                    _logger.LogWarning("Redis GetAsync failed: Key is empty");
                    return null;
                }

                var value = await _database.StringGetAsync(Key);
                
                if (value.HasValue)
                {
                    _logger.LogDebug("Redis key {Key} retrieved successfully", Key);
                    return value.ToString();
                }
                else
                {
                    _logger.LogDebug("Redis key {Key} not found", Key);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Redis key {Key}", Key);
                return null;
            }
        }

        public async Task<bool> BlacklistTokenAsync(string token, TimeSpan expiry)
        {
            try
            {
                // Validation checks
                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogWarning("BlacklistTokenAsync failed: Token is empty");
                    return false;
                }

                if (expiry <= TimeSpan.Zero)
                {
                    _logger.LogWarning("BlacklistTokenAsync failed: Invalid expiry time for token");
                    return false;
                }

                var key = $"blacklist:{token}";
                var result = await _database.StringSetAsync(key, "revoked", expiry);
                
                if (result)
                {
                    _logger.LogInformation("Token blacklisted successfully with expiry {Expiry}", expiry);
                }
                else
                {
                    _logger.LogWarning("Failed to blacklist token");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blacklisting token");
                return false;
            }
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            try
            {
                // Validation checks
                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogWarning("IsTokenBlacklistedAsync failed: Token is empty");
                    return false;
                }

                var key = $"blacklist:{token}";
                var exists = await _database.KeyExistsAsync(key);
                
                if (exists)
                {
                    _logger.LogDebug("Token found in blacklist");
                }

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if token is blacklisted");
                return false;
            }
        }

        public async Task<T?> GetObjectAsync<T>(string key) where T : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    _logger.LogWarning("Redis GetObjectAsync failed: Key is empty");
                    return null;
                }

                var value = await _database.StringGetAsync(key);
                
                if (value.HasValue)
                {
                    _logger.LogDebug("Redis key {Key} retrieved successfully", key);
                    return JsonSerializer.Deserialize<T>(value.ToString());
                }
                else
                {
                    _logger.LogDebug("Redis key {Key} not found", key);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Redis object for key {Key}", key);
                return null;
            }
        }

        public async Task<bool> SetObjectAsync<T>(string key, T value, TimeSpan expiry) where T : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    _logger.LogWarning("Redis SetObjectAsync failed: Key is empty");
                    return false;
                }

                if (value == null)
                {
                    _logger.LogWarning("Redis SetObjectAsync failed: Value is null for key {Key}", key);
                    return false;
                }

                if (expiry <= TimeSpan.Zero)
                {
                    _logger.LogWarning("Redis SetObjectAsync failed: Invalid expiry time for key {Key}", key);
                    return false;
                }

                var serializedValue = JsonSerializer.Serialize(value);
                var result = await _database.StringSetAsync(key, serializedValue, expiry);
                
                if (result)
                {
                    _logger.LogDebug("Redis object key {Key} set successfully with expiry {Expiry}", key, expiry);
                }
                else
                {
                    _logger.LogWarning("Redis SetObjectAsync failed for key {Key}", key);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting Redis object for key {Key}", key);
                return false;
            }
        }

        public async Task<bool> DeleteByPatternAsync(string pattern)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pattern))
                {
                    _logger.LogWarning("Redis DeleteByPatternAsync failed: Pattern is empty");
                    return false;
                }

                var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
                var keys = server.Keys(pattern: pattern).ToArray();
                
                if (keys.Length > 0)
                {
                    await _database.KeyDeleteAsync(keys);
                    _logger.LogDebug("Deleted {Count} keys matching pattern {Pattern}", keys.Length, pattern);
                    return true;
                }
                
                _logger.LogDebug("No keys found matching pattern {Pattern}", pattern);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Redis keys by pattern {Pattern}", pattern);
                return false;
            }
        }
    }
}