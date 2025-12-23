using KBXAdmin.Domain.Entities;
using KBXAdmin.Domain.Entities.Admin;
using Microsoft.EntityFrameworkCore;

namespace KBXAdmin.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<LoginAuditLog> LoginAuditLogs => Set<LoginAuditLog>();
    public DbSet<FormEntity> Forms => Set<FormEntity>();
    public DbSet<FormVersion> FormVersions => Set<FormVersion>();
    public DbSet<FormAuditLog> FormAuditLogs => Set<FormAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FormEntity>()
           .HasMany(f => f.Versions)
           .WithOne(v => v.Form)
           .HasForeignKey(v => v.FormId);
        // Unique email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // LoginLog Relations
        modelBuilder.Entity<LoginAuditLog>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
