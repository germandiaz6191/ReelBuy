using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;


namespace ReelBuy.Tests.Repositories;

public class StatusesRepositoryTests : IDisposable
{
    private readonly DataContext _context;
    private readonly StatusesRepository _repository;

    public StatusesRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DataContext(options);
        _repository = new StatusesRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnAllStatuses()
    {
        // Arrange
        var statuses = new List<Status>
        {
            new Status { Name = "Status 1" },
            new Status { Name = "Status 2" },
            new Status { Name = "Status 3" }
        };
        await _context.Statuses.AddRangeAsync(statuses);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal(3, result.Result.Count());
    }

    [Fact]
    public async Task GetAsync_WithId_ShouldReturnStatus()
    {
        // Arrange
        var status = new Status { Name = "Test Status" };
        await _context.Statuses.AddAsync(status);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(status.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal(status.Id, result.Result.Id);
        Assert.Equal(status.Name, result.Result.Name);
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
    public async Task GetAsync_WithPagination_ShouldReturnPaginatedStatuses()
    {
        // Arrange
        var statuses = new List<Status>();
        for (int i = 1; i <= 15; i++)
        {
            statuses.Add(new Status { Name = $"Status {i}" });
        }
        await _context.Statuses.AddRangeAsync(statuses);
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
        Assert.NotNull(result.Result);
        Assert.Equal(10, result.Result.Count());
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ShouldReturnTotalCount()
    {
        // Arrange
        var statuses = new List<Status>
        {
            new Status { Name = "Status 1" },
            new Status { Name = "Status 2" },
            new Status { Name = "Status 3" }
        };
        await _context.Statuses.AddRangeAsync(statuses);
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
    public async Task GetComboAsync_ShouldReturnAllStatuses()
    {
        // Arrange
        var statuses = new List<Status>
        {
            new Status { Name = "Status 1" },
            new Status { Name = "Status 2" },
            new Status { Name = "Status 3" }
        };
        await _context.Statuses.AddRangeAsync(statuses);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComboAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }
} 