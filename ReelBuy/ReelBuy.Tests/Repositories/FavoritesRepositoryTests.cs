using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;

namespace ReelBuy.Tests.Repositories;

public class FavoritesRepositoryTests : IDisposable
{
    private readonly DataContext _context;
    private readonly FavoritesRepository _repository;

    public FavoritesRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DataContext(options);
        _repository = new FavoritesRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnAllFavorites()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            UserName = "testuser",
            PhoneNumber = "1234567890"
        };
        await _context.Users.AddAsync(user);

        var product = new Product
        {
            Name = "Test Product",
            Price = 100.00m,
            Description = "Test Product Description"
        };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var favorite = new Favorite { UserId = user.Id, ProductId = product.Id, Name = "Test Favorite" };
        await _context.Favorites.AddAsync(favorite);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.Equal(1, result.Result.Count());
    }

    [Fact]
    public async Task GetAsync_WithId_ShouldReturnFavorite()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            UserName = "testuser",
            PhoneNumber = "1234567890"
        };
        await _context.Users.AddAsync(user);

        var product = new Product
        {
            Name = "Test Product",
            Price = 100.00m,
            Description = "Test Product Description"
        };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var favorite = new Favorite { UserId = user.Id, ProductId = product.Id, Name = "Test Favorite" };
        await _context.Favorites.AddAsync(favorite);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(favorite.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal(favorite.Id, result.Result.Id);
    }

    [Fact]
    public async Task GetAsync_WithInvalidId_ShouldReturnError()
    {
        // Act
        var result = await _repository.GetAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.WasSuccess);
        Assert.Equal("ERR001", result.Message);
    }

    [Fact]
    public async Task GetAsync_WithUserIdAndProductId_ShouldReturnFavorite()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            UserName = "testuser",
            PhoneNumber = "1234567890"
        };
        await _context.Users.AddAsync(user);

        var product = new Product
        {
            Name = "Test Product",
            Price = 100.00m,
            Description = "Test Product Description"
        };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var favorite = new Favorite { UserId = user.Id, ProductId = product.Id, Name = "Test Favorite" };
        await _context.Favorites.AddAsync(favorite);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(user.Id, product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
    }

    [Fact]
    public async Task GetAsync_WithInvalidUserIdAndProductId_ShouldReturnEmptyFavorite()
    {
        // Act
        var result = await _repository.GetAsync("invalid-user", 999);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal(0, result.Result.Id);
    }

    [Fact]
    public async Task GetAsync_WithPagination_ShouldReturnPaginatedFavorites()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            UserName = "testuser",
            PhoneNumber = "1234567890"
        };
        await _context.Users.AddAsync(user);

        var products = new List<Product>();
        var favorites = new List<Favorite>();
        for (int i = 1; i <= 15; i++)
        {
            var product = new Product
            {
                Name = $"Product {i}",
                Price = 100.00m * i,
                Description = $"Description for Product {i}"
            };
            products.Add(product);
        }
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        foreach (var product in products)
        {
            favorites.Add(new Favorite { UserId = user.Id, ProductId = product.Id, Name = $"Favorite {product.Id}" });
        }
        await _context.Favorites.AddRangeAsync(favorites);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO
        {
            Page = 1,
            RecordsNumber = 10
        };

        // Act
        var result = await _repository.GetAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.Equal(10, result.Result.Count());
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ShouldReturnTotalCount()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            UserName = "testuser",
            PhoneNumber = "1234567890"
        };
        await _context.Users.AddAsync(user);

        var products = new List<Product>
        {
            new Product { Name = "Product 1", Price = 100.00m, Description = "Description 1" },
            new Product { Name = "Product 2", Price = 200.00m, Description = "Description 2" },
            new Product { Name = "Product 3", Price = 300.00m, Description = "Description 3" }
        };
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var favorites = products.Select(p => new Favorite { UserId = user.Id, ProductId = p.Id, Name = $"Favorite {p.Id}" }).ToList();
        await _context.Favorites.AddRangeAsync(favorites);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO
        {
            Page = 1,
            RecordsNumber = 10
        };

        // Act
        var result = await _repository.GetTotalRecordsAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.Equal(3, result.Result);
    }

    [Fact]
    public async Task AddAsync_WithValidData_ShouldAddFavorite()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            UserName = "testuser",
            PhoneNumber = "1234567890"
        };
        await _context.Users.AddAsync(user);

        var product = new Product
        {
            Name = "Test Product",
            Price = 100.00m,
            Description = "Test Product Description"
        };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var favoriteDTO = new FavoriteDTO
        {
            UserId = user.Id,
            ProductId = product.Id
        };

        // Act
        var result = await _repository.AddAsync(favoriteDTO);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal(user.Id, result.Result.UserId);
        Assert.Equal(product.Id, result.Result.ProductId);
    }

    [Fact]
    public async Task AddAsync_WithInvalidData_ShouldReturnError()
    {
        // Arrange
        var favoriteDTO = new FavoriteDTO
        {
            UserId = "invalid-user",
            ProductId = 999
        };

        // Act
        var result = await _repository.AddAsync(favoriteDTO);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.WasSuccess);
        Assert.Equal("ERR009", result.Message);
    }
} 