using Microsoft.EntityFrameworkCore;
using CoreTriageAI.Models;

namespace CoreTriageAI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Complain> Complains { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Complain>()
            .Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Complain>()
            .Property(c => c.SentimentScore)
            .HasPrecision(5, 2);
    }
}
