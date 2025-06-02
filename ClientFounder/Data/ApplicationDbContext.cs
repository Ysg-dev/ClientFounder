using Microsoft.EntityFrameworkCore;
using ClientFounder.Models;

namespace ClientFounder.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Founder> Founders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Client>()
            .HasMany(c => c.Founders)
            .WithOne(f => f.Client)
            .HasForeignKey(f => f.ClientId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Client or Founder)
            {
                var now = DateTime.UtcNow.AddHours(3);
                if (entry.State == EntityState.Added)
                    entry.Property("CreatedAt").CurrentValue = now;
                entry.Property("UpdatedAt").CurrentValue = now;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}