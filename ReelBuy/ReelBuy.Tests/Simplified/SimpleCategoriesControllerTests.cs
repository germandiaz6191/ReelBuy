using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReelBuy.Backend.Controllers;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;
using Xunit;
using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;

namespace ReelBuy.Tests.Simplified
{
    public class SimpleCategoriesControllerTests
    {
        [Fact]
        public async Task GetAsync_ShouldReturnAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };

            var actionResponse = new ActionResponse<IEnumerable<Category>>
            {
                WasSuccess = true,
                Result = categories
            };

            var mockUnitOfWork = new Mock<IGenericUnitOfWork<Category>>();
            mockUnitOfWork.Setup(uow => uow.GetAsync())
                .ReturnsAsync(actionResponse);

            var controller = new GenericController<Category>(mockUnitOfWork.Object);

            // Act
            var result = await controller.GetAsync();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCategories = okResult.Value.Should().BeAssignableTo<IEnumerable<Category>>().Subject;
            returnedCategories.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetAsync_WithId_ShouldReturnCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Category 1" };

            var actionResponse = new ActionResponse<Category>
            {
                WasSuccess = true,
                Result = category
            };

            var mockUnitOfWork = new Mock<IGenericUnitOfWork<Category>>();
            mockUnitOfWork.Setup(uow => uow.GetAsync(1))
                .ReturnsAsync(actionResponse);

            var controller = new GenericController<Category>(mockUnitOfWork.Object);

            // Act
            var result = await controller.GetAsync(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCategory = okResult.Value.Should().BeAssignableTo<Category>().Subject;
            returnedCategory.Id.Should().Be(1);
            returnedCategory.Name.Should().Be("Category 1");
        }

        private DataContext GetDbContext(string testName)
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: testName)
                .Options;
            return new DataContext(options);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnOk_WhenCategoryDeleted()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IGenericUnitOfWork<Category>>();
            var mockCategoriesUnitOfWork = new Mock<ICategoriesUnitOfWork>();
            var dbContext = GetDbContext(nameof(DeleteAsync_ShouldReturnOk_WhenCategoryDeleted));
            var category = new Category { Id = 1, Name = "Category 1" };
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();
            var controller = new CategoriesController(mockUnitOfWork.Object, mockCategoriesUnitOfWork.Object);

            mockUnitOfWork.Setup(uow => uow.DeleteAsync(1))
                .ReturnsAsync(new ActionResponse<Category> { WasSuccess = true });

            // Act
            var result = await controller.DeleteAsync(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
} 