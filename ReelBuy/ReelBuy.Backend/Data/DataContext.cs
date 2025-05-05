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
    public DbSet<Reel> Reels { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Comments> Comments { get; set; }

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
        modelBuilder.Entity<Reel>().HasIndex(i => i.Name).IsUnique();
        modelBuilder.Entity<Favorite>().HasIndex(i => i.Name);
        modelBuilder.Entity<Comments>().HasIndex(i => i.Description);

        // Configuración de relaciones
        modelBuilder.Entity<Product>()
        .Property(p => p.Price)
        .HasPrecision(18, 4);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Status)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Marketplace)
            .WithMany(m => m.Products)
            .HasForeignKey(p => p.MarketplaceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reel>()
            .HasOne(r => r.Product)
            .WithMany(p => p.Reels)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Favorite>()
            .HasKey(f => f.Id);

        modelBuilder.Entity<Favorite>()
            .HasIndex(f => new { f.UserId, f.ProductId })
            .IsUnique();

        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade); // ok

        // Configuración de la relación con Product

        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.Product)
            .WithMany(p => p.Favorites)
            .HasForeignKey(f => f.ProductId)
            .OnDelete(DeleteBehavior.Restrict); // <--- evita el conflicto

        modelBuilder.Entity<Favorite>()
        .Property(f => f.Id)
        .ValueGeneratedOnAdd();

        modelBuilder.Entity<Comments>()
            .HasKey(f => new { f.UserId, f.ProductId });

        modelBuilder.Entity<Comments>()
            .HasOne(f => f.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade); // ok

        modelBuilder.Entity<Comments>()
            .HasOne(f => f.Product)
            .WithMany(p => p.Comments)
            .HasForeignKey(f => f.ProductId)
            .OnDelete(DeleteBehavior.Restrict); // <--- evita el conflicto

        modelBuilder.Entity<Store>()
            .HasOne(i => i.User)
            .WithMany(i => i.Stores)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Store>()
            .HasOne(i => i.City)
            .WithMany(i => i.Stores)
            .HasForeignKey(i => i.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>() //intermedia de forma implicita
            .HasMany(f => f.Likes)
            .WithMany(u => u.LikedBy)
            .UsingEntity<Dictionary<string, object>>(
                "UserProductLikes",
                j => j
                    .HasOne<Product>()
                    .WithMany()
                    .HasForeignKey("LikesId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("LikedById")
                    .OnDelete(DeleteBehavior.Restrict));
    }
}