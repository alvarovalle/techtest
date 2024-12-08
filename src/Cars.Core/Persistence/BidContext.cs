using Cars.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Cars.Core.Persistence;

public class BloggingContext : DbContext
{
    public DbSet<Bid> Bids { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bid>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=ms-sql;User ID=sa;Password=LltF8Nx*yo;Min Pool Size=10;Max Pool Size=100;TrustServerCertificate=True;",
            options => options.EnableRetryOnFailure());
}