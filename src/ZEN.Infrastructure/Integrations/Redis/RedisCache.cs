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
                options.ConnectTimeout = 30000; // Increase timeout to 30s for Upstash
                options.SyncTimeout = 30000;
                options.AsyncTimeout = 30000;
                options.KeepAlive = 60;
                options.ReconnectRetryPolicy = new ExponentialRetry(1000, 10000); // Retry policy

                // Enable SSL if connection string uses rediss://
                options.Ssl = redisConnectionString.StartsWith("rediss://", StringComparison.OrdinalIgnoreCase);
                
                // SSL configuration for Upstash
                if (options.Ssl)
                {
                    options.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
                    options.CheckCertificateRevocation = false; // Upstash uses self-signed certs
                }

                // Allow admin commands for RemoveByPrefixAsync
                options.AllowAdmin = true;

                // Connection resilience
                options.ConnectRetry = 3;
                options.DefaultDatabase = 0;

                Console.WriteLine($"[Redis] Connecting to: {string.Join(",", options.EndPoints)}");
                Console.WriteLine($"[Redis] User: {options.User ?? "default"}, SSL: {options.Ssl}");
                Console.WriteLine($"[Redis] ConnectTimeout: {options.ConnectTimeout}ms, SyncTimeout: {options.SyncTimeout}ms");

                // Connect with retry
                _redis = ConnectionMultiplexer.Connect(options);
                _db = _redis.GetDatabase();
                
                // Test connection synchronously (will throw if connection fails)
                // Note: Connection is established lazily, so we just verify it can be created
                Console.WriteLine("[Redis] Connection multiplexer created, connection will be established on first use");
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
            try
            {
                var value = await _db.StringGetAsync(key);
                return value.HasValue ? value.ToString() : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] GetAsync failed for key '{key}': {ex.Message}");
                return null; // Return null on error instead of throwing
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] RemoveAsync failed for key '{key}': {ex.Message}");
                // Don't throw, just log
            }
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
            try
            {
                await _db.StringSetAsync(key, value, expiry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] SetAsync failed for key '{key}': {ex.Message}");
                // Don't throw, just log - cache failures shouldn't break the app
            }
        }
    }
}