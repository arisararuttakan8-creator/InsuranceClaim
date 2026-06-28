using Microsoft.EntityFrameworkCore;
using InsuranceClaim.Models;

namespace InsuranceClaim.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }

    public DbSet<Claim> Claims { get; set; }
    public DbSet<ClaimAuditLog> ClaimAuditLogs { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Claim
        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerNo).IsRequired();
            entity.Property(e => e.ClaimTitle).IsRequired();
            entity.Property(e => e.ClaimStatus).HasDefaultValue("Pending");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });

        // AuditLog
        modelBuilder.Entity<ClaimAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Claim)
                  .WithMany(c => c.AuditLogs)
                  .HasForeignKey(e => e.ClaimId);
            entity.Property(e => e.ChangedAt).HasDefaultValueSql("NOW()");
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.Role).IsRequired();
        });
    }
}