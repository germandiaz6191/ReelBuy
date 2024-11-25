using Microsoft.EntityFrameworkCore;
using ReelBuy.Shared.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ReelBuy.Backend.Data;

public class DataContext : IdentityDbContext<User>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Country> Countries { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<Marketplace> Marketplaces { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Reputation> Reputations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Country>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Department>().HasIndex(i => i.Name).IsUnique();
        modelBuilder.Entity<City>().HasIndex(i => i.Name).IsUnique();
        modelBuilder.Entity<Store>().HasIndex(i => i.Name).IsUnique();
        modelBuilder.Entity<Product>().HasIndex(i => i.Name);
        modelBuilder.Entity<Status>().HasIndex(i => i.Name).IsUnique();
        modelBuilder.Entity<Marketplace>().HasIndex(i => i.Name).IsUnique();
        modelBuilder.Entity<Profile>().HasIndex(i => i.Name).IsUnique();
        modelBuilder.Entity<Category>().HasIndex(i => i.Name).IsUnique();
        modelBuilder.Entity<Reputation>().HasIndex(i => i.Name).IsUnique();
    }
}