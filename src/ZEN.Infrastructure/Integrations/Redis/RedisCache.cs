using StackExchange.Redis;
using ZEN.Domain.Interfaces;


namespace ZEN.Infrastructure.Integrations.Redis
{
    public class RedisCache : IRedisCache
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisCache(string redisConnectionString)
        {
            if (string.IsNullOrWhiteSpace(redisConnectionString))
            {
                throw new ArgumentException("Redis connection string cannot be null or empty", nameof(redisConnectionString));
            }

            try
            {
                // Parse connection string - support both URL format and host:port format
                ConfigurationOptions options;

                // If it's a full URL (redis:// or rediss://)
                if (redisConnectionString.StartsWith("redis://", StringComparison.OrdinalIgnoreCase) ||
                    redisConnectionString.StartsWith("rediss://", StringComparison.OrdinalIgnoreCase))
                {
                    options = ConfigurationOptions.Parse(redisConnectionString);
                }
                else
                {
                    // If it's just host:port format, parse manually
                    options = new ConfigurationOptions();
                    var parts = redisConnectionString.Split(':');
                    if (parts.Length >= 2)
                    {
                        options.EndPoints.Add(parts[0], int.Parse(parts[1]));
                    }
                    else
                    {
                        options.EndPoints.Add(redisConnectionString, 6379);
                    }
                }

                // Set user if provided
                var redisUser = Environment.GetEnvironmentVariable("REDIS_USER");
                if (!string.IsNullOrEmpty(redisUser))
                {
                    options.User = redisUser;
                }

                // Connection settings
                options.AbortOnConnectFail = false;
                options.ConnectTimeout = 10000;
                options.SyncTimeout = 10000;
                options.KeepAlive = 60;

                // Enable SSL if connection string uses rediss://
                options.Ssl = redisConnectionString.StartsWith("rediss://", StringComparison.OrdinalIgnoreCase);

                // Allow admin commands for RemoveByPrefixAsync
                options.AllowAdmin = true;

                Console.WriteLine($"[Redis] Connecting to: {string.Join(",", options.EndPoints)}");
                Console.WriteLine($"[Redis] User: {options.User ?? "default"}, SSL: {options.Ssl}");

                _redis = ConnectionMultiplexer.Connect(options);
                _db = _redis.GetDatabase();

                Console.WriteLine("[Redis] Connection established successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] Connection failed: {ex.Message}");
                Console.WriteLine($"[Redis] Connection string: {redisConnectionString}");
                throw new InvalidOperationException($"Failed to connect to Redis: {ex.Message}", ex);
            }
        }

        public async Task<string?> GetAsync(string key)
        {
            var value = await _db.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            var endpoints = _redis.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                if (!server.IsConnected) continue;
                var keys = server.Keys(pattern: $"{prefix}*").ToArray();

                foreach (var key in keys)
                {
                    await _db.KeyDeleteAsync(key);
                }
            }
        }

        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _db.StringSetAsync(key, value, expiry);
        }
    }
}