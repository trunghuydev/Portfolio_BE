
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZEN.Infrastructure.Persistence;
using ZEN.Infrastructure.Core.Mapping;
using Microsoft.EntityFrameworkCore;
using ZEN.Infrastructure.Integrations;
using AutoMapper;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using ZEN.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using ZEN.Infrastructure.Extentions;
using ZEN.Infrastructure.Mysql;
using ZEN.Infrastructure.Mysql.Persistence;
using ZEN.Domain.Services;
using ZEN.Infrastructure.InSystemProvider;
using ZEN.Domain.Definition;
using ZEN.Domain.Interfaces;
using ZEN.Infrastructure.Integrations.CloudStorage;
using ZEN.Infrastructure.Integrations.SendMail;
// using Microsoft.Extensions.Caching.StackExchangeRedis;
using ZEN.Infrastructure.Integrations.Redis;
using StackExchange.Redis;
using System.Security.Authentication;

namespace ZEN.Infrastructure;

public static class Infrastructure
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var opts = new DbContextOptionsBuilder();
        //---- Get redis connectionString -------

        // -----------------------

        if (!RuntimeConfig.IsOnPremise)
        {
            services.ApplyNeonInfrast(configuration);
            // services.ApplyMysqlInfrast(configuration);
            services.AddScoped<IUnitOfWork, UnitOfWork<AppDbContext>>();
            services.ApplyIdentityBuilder<AppDbContext>();
        }

        services.RegisterMapsterConfiguration();
        services.AddScoped<ProvinceOpenAPIService>();

        #region use for ctcore.q
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        services.AddSingleton(mapperConfig.CreateMapper());

        services.AddScoped<IHardwareSpec, HardwareSpecService>();
        services.AddScoped<IVAdminApiClient, VAdminApiClient>();
        services.AddScoped<ISavePhotoToCloud, SavePhotoToCloud>();
        services.AddScoped<ISendMail, SendMail>();

        #endregion

        // Register Redis Cache
        // Nếu REDIS_CONNECTION_STRING null hoặc empty → fallback sang NullRedisCache
        var connStr = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
        
        if (string.IsNullOrWhiteSpace(connStr))
        {
            Console.WriteLine("[Redis] No REDIS_CONNECTION_STRING provided, using NullRedisCache.");
            services.AddSingleton<IRedisCache>(new NullRedisCache());
        }
        else
        {
            Console.WriteLine($"[Redis] Connection string found: {MaskConnectionString(connStr)}");
            
            // Use factory pattern to handle connection errors gracefully
            // Initialize async at startup
            services.AddSingleton<IRedisCache>(sp =>
            {
                try
                {
                    var redisCache = new RedisCache(connStr);
                    // Initialize async at startup (block here to ensure warm-up)
                    redisCache.InitializeAsync().GetAwaiter().GetResult();
                    Console.WriteLine("[Redis] RedisCache service registered and initialized successfully");
                    return redisCache;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Redis] ERROR: Failed to initialize Redis. Using NullRedisCache.");
                    Console.WriteLine($"[Redis] Error details: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"[Redis] Inner exception: {ex.InnerException.Message}");
                    }
                    return new NullRedisCache();
                }
            });
        }
        
        // Helper method to mask sensitive connection string
        static string MaskConnectionString(string connStr)
        {
            if (string.IsNullOrEmpty(connStr)) return "empty";
            
            // Mask password in connection string
            if (connStr.Contains("@"))
            {
                var parts = connStr.Split('@');
                if (parts.Length == 2)
                {
                    var authPart = parts[0];
                    if (authPart.Contains(":"))
                    {
                        var authParts = authPart.Split(':');
                        if (authParts.Length >= 2)
                        {
                            return $"{authParts[0]}:****@{parts[1]}";
                        }
                    }
                }
            }
            
            // If it's just host:port, show as is
            return connStr;
        }
        // services.AddStackExchangeRedisCache(options =>
        //    {
        //        options.Configuration = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING")!;
        //        options.InstanceName = "Portfolio_";
        //    });

    }

    private static void ApplyIdentityBuilder<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddTransient<IEmailSender<AspUser>, NullEmailSender>();
        services.AddIdentityCore<AspUser>(opt =>
        {
            opt.User.RequireUniqueEmail = true;
            opt.SignIn.RequireConfirmedEmail = false;
            opt.SignIn.RequireConfirmedAccount = false;
            opt.Password.RequireDigit = false;
            opt.Password.RequiredLength = 3;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireLowercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<TContext>()
        .AddSignInManager();
    }
}
