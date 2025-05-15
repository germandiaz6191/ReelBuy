using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Shared.Entities;
using Xunit;

namespace ReelBuy.Tests.Simplified
{
    public class SimpleGenericRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddEntity()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: nameof(AddAsync_ShouldAddEntity))
                .Options;

            using var context = new DataContext(options);
            var repository = new GenericRepository<Category>(context);
            var category = new Category { Name = "Test Category" };

            // Act
            var result = await repository.AddAsync(category);

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Id.Should().BeGreaterThan(0);
            
            // Verify it's in the database
            var savedCategory = await context.Categories.FindAsync(result.Result.Id);
            savedCategory.Should().NotBeNull();
            savedCategory!.Name.Should().Be("Test Category");
        }

        [Fact]
        public async Task GetAsync_WithId_ShouldReturnEntity()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: nameof(GetAsync_WithId_ShouldReturnEntity))
                .Options;

            using var context = new DataContext(options);
            var repository = new GenericRepository<Category>(context);
            
            var category = new Category { Id = 1, Name = "Test Category" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAsync(1);

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.Name.Should().Be("Test Category");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntity()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: nameof(DeleteAsync_ShouldRemoveEntity))
                .Options;

            using var context = new DataContext(options);
            var repository = new GenericRepository<Category>(context);
            
            var category = new Category { Id = 1, Name = "Test Category" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.DeleteAsync(1);

            // Assert
            result.WasSuccess.Should().BeTrue();
            
            // Verify it's not in the database
            var deletedCategory = await context.Categories.FindAsync(1);
            deletedCategory.Should().BeNull();
        }
    }
} 