using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;


namespace ReelBuy.Tests.Repositories;

public class ProfilesRepositoryTests : IDisposable
{
    private DataContext _context = null!;
    private ProfilesRepository _repository = null!;

    public ProfilesRepositoryTests()
    {
        Setup();
    }

    private void Setup()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DataContext(options);
        _repository = new ProfilesRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnAllProfiles()
    {
        // Arrange
        var profiles = new List<Profile>
        {
            new() { Name = "Perfil 1" },
            new() { Name = "Perfil 2" },
            new() { Name = "Perfil 3" }
        };
        await _context.Profiles.AddRangeAsync(profiles);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal(3, result.Result.Count());
        Assert.All(result.Result, profile => Assert.NotNull(profile));
    }

    [Fact]
    public async Task GetAsync_WithId_ShouldReturnProfile()
    {
        // Arrange
        var profile = new Profile
        {
            Name = "Perfil 1"
        };
        await _context.Profiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(profile.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal(profile.Id, result.Result.Id);
        Assert.Equal(profile.Name, result.Result.Name);
    }

    [Fact]
    public async Task GetAsync_WithInvalidId_ShouldReturnError()
    {
        // Act
        var result = await _repository.GetAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.WasSuccess);
        Assert.Null(result.Result);
        Assert.Equal("ERR001", result.Message);
    }

    [Fact]
    public async Task GetAsync_WithPagination_ShouldReturnPaginatedProfiles()
    {
        // Arrange
        var profiles = new List<Profile>
        {
            new() { Name = "Perfil 1" },
            new() { Name = "Perfil 2" },
            new() { Name = "Perfil 3" },
            new() { Name = "Perfil 4" },
            new() { Name = "Perfil 5" }
        };
        await _context.Profiles.AddRangeAsync(profiles);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 2 };

        // Act
        var result = await _repository.GetAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal(2, result.Result.Count());
        Assert.All(result.Result, profile => Assert.NotNull(profile));
    }

    [Fact]
    public async Task GetAsync_WithPaginationAndFilter_ShouldReturnFilteredProfiles()
    {
        // Arrange
        var profiles = new List<Profile>
        {
            new() { Name = "Perfil A" },
            new() { Name = "Perfil B" },
            new() { Name = "Perfil C" }
        };
        await _context.Profiles.AddRangeAsync(profiles);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10, Filter = "A" };

        // Act
        var result = await _repository.GetAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Single(result.Result);
        Assert.Equal("Perfil A", result.Result.First().Name);
        Assert.All(result.Result, profile => Assert.NotNull(profile));
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ShouldReturnTotalCount()
    {
        // Arrange
        var profiles = new List<Profile>
        {
            new() { Name = "Perfil 1" },
            new() { Name = "Perfil 2" },
            new() { Name = "Perfil 3" }
        };
        await _context.Profiles.AddRangeAsync(profiles);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO { Filter = "" };

        // Act
        var result = await _repository.GetTotalRecordsAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.Equal(3, result.Result);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_WithFilter_ShouldReturnFilteredCount()
    {
        // Arrange
        var profiles = new List<Profile>
        {
            new() { Name = "Perfil A" },
            new() { Name = "Perfil B" },
            new() { Name = "Perfil C" }
        };
        await _context.Profiles.AddRangeAsync(profiles);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO { Filter = "A" };

        // Act
        var result = await _repository.GetTotalRecordsAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.Equal(1, result.Result);
    }

    [Fact]
    public async Task GetComboAsync_ShouldReturnNonAdminProfiles()
    {
        // Arrange
        var profiles = new List<Profile>
        {
            new() { Name = "Admin" },
            new() { Name = "User" },
            new() { Name = "Guest" }
        };
        await _context.Profiles.AddRangeAsync(profiles);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComboAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.DoesNotContain(result, p => p.Name.Contains("Admin"));
    }

    [Fact]
    public async Task GetAsync_WithPagination_ShouldReturnEmptyList_WhenNoProfiles()
    {
        // Arrange
        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };

        // Act
        var result = await _repository.GetAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ShouldReturnZero_WhenNoProfiles()
    {
        // Arrange
        var pagination = new PaginationDTO { Filter = "" };

        // Act
        var result = await _repository.GetTotalRecordsAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.Equal(0, result.Result);
    }
} 