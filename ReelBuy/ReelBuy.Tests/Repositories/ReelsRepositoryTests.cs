using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;

namespace ReelBuy.Tests.Repositories;

public class ReelsRepositoryTests : IDisposable
{
    private readonly DataContext _context;
    private readonly ReelsRepository _repository;

    public ReelsRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DataContext(options);
        _repository = new ReelsRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnAllReels()
    {
        // Arrange
        var reels = new List<Reel>
        {
            new Reel { Name = "Reel 1", ReelUri = "https://example.com/reel1.mp4" },
            new Reel { Name = "Reel 2", ReelUri = "https://example.com/reel2.mp4" },
            new Reel { Name = "Reel 3", ReelUri = "https://example.com/reel3.mp4" }
        };
        await _context.Reels.AddRangeAsync(reels);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.Equal(3, result.Result?.Count() ?? 0);
    }

    [Fact]
    public async Task GetAsync_WithId_ShouldReturnReel()
    {
        // Arrange
        var reel = new Reel
        {
            Name = "Test Reel",
            ReelUri = "https://example.com/test.mp4"
        };
        await _context.Reels.AddAsync(reel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(reel.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal(reel.Id, result.Result.Id);
        Assert.Equal(reel.Name, result.Result.Name);
        Assert.Equal(reel.ReelUri, result.Result.ReelUri);
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
    public async Task GetAsync_WithPagination_ShouldReturnPaginatedReels()
    {
        // Arrange
        var reels = new List<Reel>();
        for (int i = 1; i <= 15; i++)
        {
            reels.Add(new Reel
            {
                Name = $"Reel {i}",
                ReelUri = $"https://example.com/reel{i}.mp4"
            });
        }
        await _context.Reels.AddRangeAsync(reels);
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
        var reels = new List<Reel>
        {
            new Reel { Name = "Reel 1", ReelUri = "https://example.com/reel1.mp4" },
            new Reel { Name = "Reel 2", ReelUri = "https://example.com/reel2.mp4" },
            new Reel { Name = "Reel 3", ReelUri = "https://example.com/reel3.mp4" }
        };
        await _context.Reels.AddRangeAsync(reels);
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
    public async Task GetComboAsync_ShouldReturnAllReels()
    {
        // Arrange
        var reels = new List<Reel>
        {
            new Reel { Name = "Reel 1", ReelUri = "https://example.com/reel1.mp4" },
            new Reel { Name = "Reel 2", ReelUri = "https://example.com/reel2.mp4" },
            new Reel { Name = "Reel 3", ReelUri = "https://example.com/reel3.mp4" }
        };
        await _context.Reels.AddRangeAsync(reels);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComboAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task AddAsync_WithValidData_ShouldAddReel()
    {
        // Arrange
        var reel = new Reel
        {
            Name = "New Reel",
            ReelUri = "https://example.com/new.mp4"
        };

        // Act
        var result = await _repository.AddAsync(reel);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);

        var addedReel = await _context.Reels.FirstOrDefaultAsync(r => r.Name == reel.Name);
        Assert.NotNull(addedReel);
        Assert.Equal(reel.ReelUri, addedReel.ReelUri);
    }

    [Fact]
    public async Task AddAsync_WithInvalidData_ShouldReturnError()
    {
        // Arrange
        var reel = new Reel { Name = "" };

        // Act
        var result = await _repository.AddAsync(reel);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.WasSuccess);
        Assert.Equal("ERR003", result.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateReel()
    {
        // Arrange
        var reel = new Reel
        {
            Name = "Original Name",
            ReelUri = "https://example.com/original.mp4"
        };
        await _context.Reels.AddAsync(reel);
        await _context.SaveChangesAsync();

        reel.Name = "Updated Name";
        reel.ReelUri = "https://example.com/updated.mp4";

        // Act
        var result = await _repository.UpdateAsync(reel);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);

        var updatedReel = await _context.Reels.FindAsync(reel.Id);
        Assert.NotNull(updatedReel);
        Assert.Equal("Updated Name", updatedReel.Name);
        Assert.Equal("https://example.com/updated.mp4", updatedReel.ReelUri);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnError()
    {
        // Arrange
        var reel = new Reel { Id = 999, Name = "Test Reel" };

        // Act
        var result = await _repository.UpdateAsync(reel);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.WasSuccess);
        Assert.Equal("ERR003", result.Message);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteReel()
    {
        // Arrange
        var reel = new Reel
        {
            Name = "Test Reel",
            ReelUri = "https://example.com/test.mp4"
        };
        await _context.Reels.AddAsync(reel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(reel.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);

        var deletedReel = await _context.Reels.FindAsync(reel.Id);
        Assert.Null(deletedReel);
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
        Assert.False(result.WasSuccess);
        Assert.Equal("ERR001", result.Message);
    }
} 