using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;

namespace ReelBuy.Tests.Repositories;

public class CountriesRepositoryTests : IDisposable
{
    private DataContext _context = null!;
    private CountriesRepository _repository = null!;

    public CountriesRepositoryTests()
    {
        Setup();
    }

    private void Setup()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DataContext(options);
        _repository = new CountriesRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnAllCountries()
    {
        // Arrange
        var countries = new List<Country>
        {
            new() { Name = "País 1" },
            new() { Name = "País 2" },
            new() { Name = "País 3" }
        };
        await _context.Countries.AddRangeAsync(countries);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync();

        // Assert
        Assert.True(result.WasSuccess);
        Assert.Equal(3, result.Result.Count());
    }

    [Fact]
    public async Task GetAsync_WithId_ShouldReturnCountry()
    {
        // Arrange
        var country = new Country { Name = "País 1" };
        await _context.Countries.AddAsync(country);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(country.Id);

        // Assert
        Assert.True(result.WasSuccess);
        Assert.Equal(country.Name, result.Result.Name);
    }

    [Fact]
    public async Task GetAsync_WithInvalidId_ShouldReturnError()
    {
        // Act
        var result = await _repository.GetAsync(999);

        // Assert
        Assert.False(result.WasSuccess);
        Assert.Equal("ERR001", result.Message);
    }

    [Fact]
    public async Task GetComboAsync_ShouldReturnOrderedCountries()
    {
        // Arrange
        var countries = new List<Country>
        {
            new() { Name = "País C" },
            new() { Name = "País A" },
            new() { Name = "País B" }
        };
        await _context.Countries.AddRangeAsync(countries);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComboAsync();

        // Assert
        Assert.Equal(3, result.Count());
        Assert.Equal("País A", result.First().Name);
    }

    [Fact]
    public async Task GetAsync_WithPagination_ShouldReturnPaginatedCountries()
    {
        // Arrange
        var countries = new List<Country>
        {
            new() { Name = "País 1" },
            new() { Name = "País 2" },
            new() { Name = "País 3" },
            new() { Name = "País 4" },
            new() { Name = "País 5" }
        };
        await _context.Countries.AddRangeAsync(countries);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 2 };

        // Act
        var result = await _repository.GetAsync(pagination);

        // Assert
        Assert.True(result.WasSuccess);
        Assert.Equal(2, result.Result.Count());
    }

    [Fact]
    public async Task GetAsync_WithPaginationAndFilter_ShouldReturnFilteredCountries()
    {
        // Arrange
        var countries = new List<Country>
        {
            new() { Name = "País A" },
            new() { Name = "País B" },
            new() { Name = "País C" }
        };
        await _context.Countries.AddRangeAsync(countries);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO
        {
            Page = 1,
            RecordsNumber = 10,
            Filter = "País A"
        };

        // Act
        var result = await _repository.GetAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.Single(result.Result);
        Assert.Equal("País A", result.Result.First().Name);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ShouldReturnTotalCount()
    {
        // Arrange
        var countries = new List<Country>
        {
            new() { Name = "País 1" },
            new() { Name = "País 2" },
            new() { Name = "País 3" }
        };
        await _context.Countries.AddRangeAsync(countries);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO { Filter = "" };

        // Act
        var result = await _repository.GetTotalRecordsAsync(pagination);

        // Assert
        Assert.True(result.WasSuccess);
        Assert.Equal(3, result.Result);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_WithFilter_ShouldReturnFilteredCount()
    {
        // Arrange
        var countries = new List<Country>
        {
            new() { Name = "País A" },
            new() { Name = "País B" },
            new() { Name = "País C" }
        };
        await _context.Countries.AddRangeAsync(countries);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO
        {
            Page = 1,
            RecordsNumber = 10,
            Filter = "País A"
        };

        // Act
        var result = await _repository.GetTotalRecordsAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.Equal(1, result.Result);
    }

    [Fact]
    public async Task AddAsync_WithValidData_ShouldAddCountry()
    {
        // Arrange
        var country = new Country { Name = "Nuevo País" };

        // Act
        var result = await _repository.AddAsync(country);

        // Assert
        Assert.True(result.WasSuccess);
        Assert.Equal("Nuevo País", result.Result.Name);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateCountry()
    {
        // Arrange
        var country = new Country { Name = "País Original" };
        await _context.Countries.AddAsync(country);
        await _context.SaveChangesAsync();

        country.Name = "País Actualizado";

        // Act
        var result = await _repository.UpdateAsync(country);

        // Assert
        Assert.True(result.WasSuccess);
        Assert.Equal("País Actualizado", result.Result.Name);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteCountry()
    {
        // Arrange
        var country = new Country { Name = "País a Eliminar" };
        await _context.Countries.AddAsync(country);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(country.Id);

        // Assert
        Assert.True(result.WasSuccess);
        Assert.Null(await _context.Countries.FindAsync(country.Id));
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnError()
    {
        // Act
        var result = await _repository.DeleteAsync(999);

        // Assert
        Assert.NotNull(result);
    }
} 