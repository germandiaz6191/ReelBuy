using Microsoft.EntityFrameworkCore;
using ReelBuy.Shared.Entities;

namespace ReelBuy.Backend.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<Marketplace> Marketplaces { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>().HasIndex(i => i.Name);
        modelBuilder.Entity<Status>().HasIndex(i => i.Name).IsUnique();
        modelBuilder.Entity<Marketplace>().HasIndex(i => i.Name).IsUnique();
    }
}