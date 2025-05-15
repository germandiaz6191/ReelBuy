using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Shared.Entities;


namespace ReelBuy.Tests.Repositories
{
    public class GenericRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetAsync_ShouldReturnAllEntities()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(GetAsync_ShouldReturnAllEntities));
            
            var category1 = new Category { Id = 1, Name = "Category 1" };
            var category2 = new Category { Id = 2, Name = "Category 2" };
            
            dbContext.Categories.Add(category1);
            dbContext.Categories.Add(category2);
            await dbContext.SaveChangesAsync();
            
            var repository = new GenericRepository<Category>(dbContext);
            
            // Act
            var result = await repository.GetAsync();
            
            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().HaveCount(2);
            result.Result.Should().Contain(c => c.Name == "Category 1");
            result.Result.Should().Contain(c => c.Name == "Category 2");
        }
        
        [Fact]
        public async Task GetAsync_ById_ShouldReturnCorrectEntity()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(GetAsync_ById_ShouldReturnCorrectEntity));
            
            var category1 = new Category { Id = 1, Name = "Category 1" };
            var category2 = new Category { Id = 2, Name = "Category 2" };
            
            dbContext.Categories.Add(category1);
            dbContext.Categories.Add(category2);
            await dbContext.SaveChangesAsync();
            
            var repository = new GenericRepository<Category>(dbContext);
            
            // Act
            var result = await repository.GetAsync(1);
            
            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.Name.Should().Be("Category 1");
        }
        
        [Fact]
        public async Task GetAsync_NonExistingId_ShouldReturnNotSuccess()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(GetAsync_NonExistingId_ShouldReturnNotSuccess));
            var repository = new GenericRepository<Category>(dbContext);
            
            // Act
            var result = await repository.GetAsync(999);
            
            // Assert
            result.WasSuccess.Should().BeFalse();
            result.Result.Should().BeNull();
            result.Message.Should().NotBeNullOrEmpty();
        }
        
        [Fact]
        public async Task AddAsync_ShouldAddEntityAndReturnIt()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(AddAsync_ShouldAddEntityAndReturnIt));
            var repository = new GenericRepository<Category>(dbContext);
            var category = new Category { Name = "New Category" };
            
            // Act
            var result = await repository.AddAsync(category);
            
            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.Id.Should().BeGreaterThan(0);
            result.Result.Name.Should().Be("New Category");
            
            var savedCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "New Category");
            savedCategory.Should().NotBeNull();
        }
        
        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(UpdateAsync_ShouldUpdateEntity));
            
            var category = new Category { Id = 1, Name = "Category 1" };
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();
            
            var repository = new GenericRepository<Category>(dbContext);
            
            // Update the entity
            category.Name = "Updated Category";
            
            // Act
            var result = await repository.UpdateAsync(category);
            
            // Assert
            result.WasSuccess.Should().BeTrue();
            
            var updatedCategory = await dbContext.Categories.FindAsync(1);
            updatedCategory.Should().NotBeNull();
            updatedCategory!.Name.Should().Be("Updated Category");
        }
        
        [Fact]
        public async Task DeleteAsync_ShouldDeleteEntity()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(DeleteAsync_ShouldDeleteEntity));
            
            var category = new Category { Id = 1, Name = "Category 1" };
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();
            
            var repository = new GenericRepository<Category>(dbContext);
            
            // Act
            var result = await repository.DeleteAsync(1);
            
            // Assert
            result.WasSuccess.Should().BeTrue();
            
            var deletedCategory = await dbContext.Categories.FindAsync(1);
            deletedCategory.Should().BeNull();
        }
        
        [Fact]
        public async Task DeleteAsync_NonExistingId_ShouldReturnNotSuccess()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(DeleteAsync_NonExistingId_ShouldReturnNotSuccess));
            var repository = new GenericRepository<Category>(dbContext);
            
            // Act
            var result = await repository.DeleteAsync(999);
            
            // Assert
            result.WasSuccess.Should().BeFalse();
            result.Message.Should().NotBeNullOrEmpty();
        }
    }
} 