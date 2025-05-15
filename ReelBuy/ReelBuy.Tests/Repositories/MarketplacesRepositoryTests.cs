using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;

namespace ReelBuy.Tests.Repositories;

public class MarketplacesRepositoryTests : IDisposable
{
    private readonly DataContext _context;
    private readonly MarketplacesRepository _repository;

    public MarketplacesRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DataContext(options);
        _repository = new MarketplacesRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnAllMarketplaces()
    {
        // Arrange
        var marketplaces = new List<Marketplace>
        {
            new Marketplace { Name = "Marketplace 1", Domain = "marketplace1.com" },
            new Marketplace { Name = "Marketplace 2", Domain = "marketplace2.com" },
            new Marketplace { Name = "Marketplace 3", Domain = "marketplace3.com" }
        };
        await _context.Marketplaces.AddRangeAsync(marketplaces);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.Equal(3, result.Result?.Count() ?? 0);
    }

    [Fact]
    public async Task GetAsync_WithId_ShouldReturnMarketplace()
    {
        // Arrange
        var marketplace = new Marketplace
        {
            Name = "Test Marketplace",
            Domain = "test.com"
        };
        await _context.Marketplaces.AddAsync(marketplace);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(marketplace.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal(marketplace.Id, result.Result.Id);
        Assert.Equal(marketplace.Name, result.Result.Name);
        Assert.Equal(marketplace.Domain, result.Result.Domain);
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
    public async Task GetAsync_WithPagination_ShouldReturnPaginatedMarketplaces()
    {
        // Arrange
        var marketplaces = new List<Marketplace>();
        for (int i = 1; i <= 15; i++)
        {
            marketplaces.Add(new Marketplace
            {
                Name = $"Marketplace {i}",
                Domain = $"marketplace{i}.com"
            });
        }
        await _context.Marketplaces.AddRangeAsync(marketplaces);
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
        Assert.Equal(10, result.Result?.Count() ?? 0);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ShouldReturnTotalCount()
    {
        // Arrange
        var marketplaces = new List<Marketplace>
        {
            new Marketplace { Name = "Marketplace 1", Domain = "marketplace1.com" },
            new Marketplace { Name = "Marketplace 2", Domain = "marketplace2.com" },
            new Marketplace { Name = "Marketplace 3", Domain = "marketplace3.com" }
        };
        await _context.Marketplaces.AddRangeAsync(marketplaces);
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
    public async Task GetComboAsync_ShouldReturnAllMarketplaces()
    {
        // Arrange
        var marketplaces = new List<Marketplace>
        {
            new Marketplace { Name = "Marketplace 1", Domain = "marketplace1.com" },
            new Marketplace { Name = "Marketplace 2", Domain = "marketplace2.com" },
            new Marketplace { Name = "Marketplace 3", Domain = "marketplace3.com" }
        };
        await _context.Marketplaces.AddRangeAsync(marketplaces);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComboAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task AddAsync_WithValidData_ShouldAddMarketplace()
    {
        // Arrange
        var marketplace = new Marketplace
        {
            Name = "New Marketplace",
            Domain = "newmarketplace.com"
        };

        // Act
        var result = await _repository.AddAsync(marketplace);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);

        var addedMarketplace = await _context.Marketplaces.FirstOrDefaultAsync(m => m.Name == marketplace.Name);
        Assert.NotNull(addedMarketplace);
        Assert.Equal(marketplace.Domain, addedMarketplace.Domain);
    }

    [Fact]
    public async Task AddAsync_WithInvalidData_ShouldReturnError()
    {
        // Arrange
        var marketplace = new Marketplace { Name = "" };

        // Act
        var result = await _repository.AddAsync(marketplace);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateMarketplace()
    {
        // Arrange
        var marketplace = new Marketplace
        {
            Name = "Original Name",
            Domain = "original.com"
        };
        await _context.Marketplaces.AddAsync(marketplace);
        await _context.SaveChangesAsync();

        marketplace.Name = "Updated Name";
        marketplace.Domain = "updated.com";

        // Act
        var result = await _repository.UpdateAsync(marketplace);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);

        var updatedMarketplace = await _context.Marketplaces.FindAsync(marketplace.Id);
        Assert.NotNull(updatedMarketplace);
        Assert.Equal("Updated Name", updatedMarketplace.Name);
        Assert.Equal("updated.com", updatedMarketplace.Domain);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnError()
    {
        // Arrange
        var marketplace = new Marketplace { Id = 999, Name = "Test Marketplace" };

        // Act
        var result = await _repository.UpdateAsync(marketplace);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteMarketplace()
    {
        // Arrange
        var marketplace = new Marketplace
        {
            Name = "Test Marketplace",
            Domain = "test.com"
        };
        await _context.Marketplaces.AddAsync(marketplace);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(marketplace.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);

        var deletedMarketplace = await _context.Marketplaces.FindAsync(marketplace.Id);
        Assert.Null(deletedMarketplace);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnError()
    {
        // Arrange
        var invalidId = 999;

        // Act
        var result = await _repository.DeleteAsync(invalidId);

        // Assert
        Assert.NotNull(result);
    }
} 