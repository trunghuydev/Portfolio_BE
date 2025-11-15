using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ZEN.Domain.Entities;
using ZEN.Domain.Entities.Identities;
using Microsoft.Extensions.Logging;
// using ZEN.Domain.Entities.Offices;

namespace ZEN.Infrastructure.Mysql.Persistence;

public class AppDbContext : IdentityDbContext<AspUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }


    /// <summary>
    /// For migrations
    /// </summary> 
    public AppDbContext()
    {
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<UserProject> UserProjects { get; set; }
    public DbSet<UserSkill> UserSkills { get; set; }
    public DbSet<WorkExperience> WorkExperiences { get; set; }
    public DbSet<Tech> Teches { get; set; }
    public DbSet<MyTask> MyTasks { get; set; }
    public DbSet<Certificate> Certificates { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        var cascadeFKs = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        RemovePluralizingTableNameConvention(modelBuilder);

        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.Skill)
                .WithMany()
                .HasForeignKey(us => us.skill_id)
                .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Certificate>()
                .ToTable("CERTIFICATE")
                .HasOne(c => c.AspUser)
                .WithMany(u => u.Certificates)
                .HasForeignKey(c => c.user_id)
                .OnDelete(DeleteBehavior.Cascade);

        // Configure username index and constraints for dynamic routing
        modelBuilder.Entity<AspUser>()
            .HasIndex(u => u.username)
            .IsUnique()
            .HasFilter("\"username\" IS NOT NULL");

        modelBuilder.Entity<AspUser>()
            .HasIndex(u => new { u.username, u.is_public })
            .HasFilter("\"username\" IS NOT NULL");

        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new AspUserConfiguration());
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                    ?? "Host=ep-weathered-poetry-a1cx5ew8-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_p1YmOxh9PtEc;SSL Mode=Require";

            optionsBuilder.UseNpgsql(connectionString, x =>
                        x.MigrationsAssembly("ZEN.Infrastructure.Mysql"))
                        .EnableSensitiveDataLogging()
                        .LogTo(Console.WriteLine, LogLevel.Warning);
        }
    }
    private static void RemovePluralizingTableNameConvention(ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.DisplayName().ToUpper());
        }
    }
}