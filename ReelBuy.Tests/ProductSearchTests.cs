using Xunit;
using ReelBuy.Frontend.Layout;
using ReelBuy.Frontend.Repositories;
using System.Threading.Tasks;
using System.Threading;

namespace ReelBuy.Tests;

public class ProductSearchTests
{
    [Fact]
    public async Task SearchProduct_WithValidQuery_ReturnsResults()
    {
        // Arrange
        var mainLayout = new MainLayout();
        var repository = new Repository();
        mainLayout.Repository = repository;

        // Act
        var results = await mainLayout.SearchProduct("test", CancellationToken.None);

        // Assert
        Assert.NotNull(results);
        Assert.NotEmpty(results);
    }

    [Fact]
    public async Task SearchProduct_WithEmptyQuery_ReturnsEmptyResults()
    {
        // Arrange
        var mainLayout = new MainLayout();
        var repository = new Repository();
        mainLayout.Repository = repository;

        // Act
        var results = await mainLayout.SearchProduct("", CancellationToken.None);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchProduct_WithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var mainLayout = new MainLayout();
        var repository = new Repository();
        mainLayout.Repository = repository;

        // Act
        var results = await mainLayout.SearchProduct("test@#$%", CancellationToken.None);

        // Assert
        Assert.NotNull(results);
    }
} 