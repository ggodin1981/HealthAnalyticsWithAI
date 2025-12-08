using Carelytix.HealthAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carelytix.HealthAnalytics.Infrastructure.Data;

public class HealthAnalyticsDbContext : DbContext
{
    public HealthAnalyticsDbContext(DbContextOptions<HealthAnalyticsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Encounter> Encounters => Set<Encounter>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(e =>
        {
            e.ToTable("Patients");
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.Mrn).IsUnique();
            e.Property(p => p.Mrn).IsRequired().HasMaxLength(50);
            e.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            e.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            e.Property(p => p.Gender).IsRequired().HasMaxLength(20);
        });

        modelBuilder.Entity<Encounter>(e =>
        {
            e.ToTable("Encounters");
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.PatientId, x.EncounterDate });
            e.Property(x => x.EncounterType).HasMaxLength(50);
            e.Property(x => x.DiagnosisCode).HasMaxLength(20);

            e.HasOne(x => x.Patient)
                .WithMany(p => p.Encounters)
                .HasForeignKey(x => x.PatientId);
        });

        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.UserName).IsUnique();
            e.Property(u => u.UserName).IsRequired().HasMaxLength(100);
            e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
            e.Property(u => u.Role).IsRequired().HasMaxLength(50);
        });

        // Seed a demo admin user with precomputed hash of "Admin123!"
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = adminId,
            UserName = "admin",
            PasswordHash = "2c5f144c7c04a1e35283cd8b67f52c16e491061b85d34ed2a5b3720d9e740382",
            Role = "Admin",
            IsActive = true
        });

        base.OnModelCreating(modelBuilder);
    }
}
