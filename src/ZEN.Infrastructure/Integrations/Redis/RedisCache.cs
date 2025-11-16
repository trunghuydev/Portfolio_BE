using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using ZEN.Domain.Interfaces;

namespace ZEN.Infrastructure.Integrations.Redis
{
    public class RedisCache : IRedisCache, IAsyncDisposable
    {
        private ConnectionMultiplexer? _redis;
        private IDatabase? _db;
        private readonly string _connectionString;

        public RedisCache(string redisConnectionString)
        {
            if (string.IsNullOrWhiteSpace(redisConnectionString))
                throw new ArgumentException("Redis connection string cannot be null or empty", nameof(redisConnectionString));

            _connectionString = redisConnectionString;
        }

        // Async initializer — gọi ở startup: await redisCache.InitializeAsync();
        public async Task InitializeAsync()
        {
            try
            {
                var options = ParseRedisUrlManually(_connectionString);

                // Connection resilience settings
                options.AbortOnConnectFail = false;
                options.ConnectRetry = 5;
                options.ConnectTimeout = 30000;
                options.SyncTimeout = 30000;
                options.AsyncTimeout = 30000;
                options.KeepAlive = 60;
                options.AllowAdmin = false; // Upstash may not allow admin commands
                options.ReconnectRetryPolicy = new ExponentialRetry(1000, 10000);
                options.DefaultDatabase = 0;

                // Log connection info (hide password)
                var endpoints = string.Join(",", options.EndPoints.Select(e => e.ToString()));
                Console.WriteLine($"[Redis] Connecting to: {endpoints} SSL:{options.Ssl} User:{(string.IsNullOrEmpty(options.User) ? "default" : options.User)}");

                // Connect async (preferred)
                _redis = await ConnectionMultiplexer.ConnectAsync(options);
                _db = _redis.GetDatabase();

                // Warm-up ping to ensure connections are established
                try
                {
                    var ping = await _db.PingAsync();
                    Console.WriteLine($"[Redis] Ping: {ping.TotalMilliseconds} ms");
                }
                catch (Exception pingEx)
                {
                    Console.WriteLine($"[Redis] Ping failed: {pingEx.Message}");
                }

                Console.WriteLine("[Redis] Connected successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] Initialize failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Parse Redis connection string manually (KHÔNG dùng ConfigurationOptions.Parse)
        /// Hỗ trợ format Upstash: rediss://user:password@host:port
        /// </summary>
        private static ConfigurationOptions ParseRedisUrlManually(string redisConnectionString)
        {
            var options = new ConfigurationOptions();

            // Nếu là URL format (rediss:// hoặc redis://)
            if (redisConnectionString.StartsWith("rediss://", StringComparison.OrdinalIgnoreCase) ||
                redisConnectionString.StartsWith("redis://", StringComparison.OrdinalIgnoreCase))
            {
                // Tách scheme
                var isSsl = redisConnectionString.StartsWith("rediss://", StringComparison.OrdinalIgnoreCase);
                options.Ssl = isSsl;

                // Bỏ scheme prefix
                var withoutScheme = redisConnectionString.Substring(redisConnectionString.IndexOf("://") + 3);

                // Tách user:password@host:port
                string? user = null;
                string? password = null;
                string host;
                int port = 6379; // default port

                // Kiểm tra có @ không (có authentication)
                if (withoutScheme.Contains("@"))
                {
                    var parts = withoutScheme.Split('@', 2);
                    var authPart = parts[0]; // user:password hoặc :password hoặc password
                    var hostPortPart = parts[1]; // host:port

                    // Parse authentication
                    if (authPart.Contains(":"))
                    {
                        var authParts = authPart.Split(':', 2);
                        if (authParts.Length == 2)
                        {
                            // Có cả user và password: user:password
                            user = string.IsNullOrWhiteSpace(authParts[0]) ? null : authParts[0];
                            password = authParts[1];
                        }
                        else
                        {
                            // Chỉ có password: :password
                            password = authParts[0];
                        }
                    }
                    else
                    {
                        // Chỉ có password (không có user)
                        password = authPart;
                    }

                    // Parse host:port
                    if (hostPortPart.Contains(":"))
                    {
                        var hostPortParts = hostPortPart.Split(':');
                        if (hostPortParts.Length >= 2 && int.TryParse(hostPortParts[hostPortParts.Length - 1], out var parsedPort))
                        {
                            // host:port
                            host = string.Join(":", hostPortParts.Take(hostPortParts.Length - 1));
                            port = parsedPort;
                        }
                        else
                        {
                            // host (không parse được port)
                            host = hostPortPart;
                        }
                    }
                    else
                    {
                        // Chỉ có host (default port)
                        host = hostPortPart;
                    }
                }
                else
                {
                    // Không có authentication: host:port hoặc host
                    if (withoutScheme.Contains(":"))
                    {
                        var hostPortParts = withoutScheme.Split(':');
                        if (hostPortParts.Length >= 2 && int.TryParse(hostPortParts[hostPortParts.Length - 1], out var parsedPort))
                        {
                            host = string.Join(":", hostPortParts.Take(hostPortParts.Length - 1));
                            port = parsedPort;
                        }
                        else
                        {
                            host = withoutScheme;
                        }
                    }
                    else
                    {
                        host = withoutScheme;
                    }
                }

                // Set values vào options
                options.EndPoints.Add(host, port);
                if (!string.IsNullOrEmpty(password))
                {
                    options.Password = password;
                }
                if (!string.IsNullOrEmpty(user))
                {
                    options.User = user;
                }
                else
                {
                    // Mặc định user = "default" cho Upstash
                    options.User = "default";
                }
            }
            else
            {
                // Format đơn giản: host:port hoặc host
                if (redisConnectionString.Contains(":"))
                {
                    var parts = redisConnectionString.Split(':');
                    if (parts.Length >= 2 && int.TryParse(parts[parts.Length - 1], out var parsedPort))
                    {
                        var host = string.Join(":", parts.Take(parts.Length - 1));
                        options.EndPoints.Add(host, parsedPort);
                    }
                    else
                    {
                        options.EndPoints.Add(redisConnectionString, 6379);
                    }
                }
                else
                {
                    options.EndPoints.Add(redisConnectionString, 6379);
                }

                // Không có SSL cho format đơn giản
                options.Ssl = false;
                options.User = "default";
            }

            return options;
        }

        public async Task<string?> GetAsync(string key)
        {
            if (_db == null) return null;
            try
            {
                var value = await _db.StringGetAsync(key);
                return value.HasValue ? value.ToString() : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] GetAsync failed for key '{key}': {ex.Message}");
                return null;
            }
        }

        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            if (_db == null) return;
            try
            {
                await _db.StringSetAsync(key, value, expiry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] SetAsync failed for key '{key}': {ex.Message}");
            }
        }

        public async Task RemoveAsync(string key)
        {
            if (_db == null) return;
            try
            {
                await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] RemoveAsync failed for key '{key}': {ex.Message}");
            }
        }

        // Safe RemoveByPrefix - Upstash/serverless Redis may not support KEYS/SCAN
        // Fallback: chỉ log warning nếu không thể thực hiện
        public async Task RemoveByPrefixAsync(string prefix)
        {
            if (_redis == null || _db == null) return;

            try
            {
                var endpoints = _redis.GetEndPoints();
                foreach (var endpoint in endpoints)
                {
                    var server = _redis.GetServer(endpoint);
                    if (server == null || !server.IsConnected) continue;

                    // Try to use SCAN via server.Keys (may not work on Upstash)
                    try
                    {
                        var keys = server.Keys(pattern: $"{prefix}*").ToArray();
                        foreach (var key in keys)
                        {
                            await _db.KeyDeleteAsync(key);
                        }
                    }
                    catch (Exception scanEx)
                    {
                        Console.WriteLine($"[Redis] RemoveByPrefixAsync: Server does not support KEYS/SCAN. Prefix '{prefix}' not cleared. Error: {scanEx.Message}");
                        Console.WriteLine("[Redis] Consider tracking keys in a Set/List for serverless Redis.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] RemoveByPrefixAsync failed: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_redis != null)
            {
                await _redis.CloseAsync();
                _redis.Dispose();
                _redis = null;
            }
        }
    }
}

