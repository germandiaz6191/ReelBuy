using Microsoft.AspNetCore.Mvc;
using Moq;
using ReelBuy.Backend.Controllers;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Tests.Controllers;

public class GenericControllerTests
{
    private readonly Mock<IGenericUnitOfWork<TestEntity>> _mockUnitOfWork;
    private readonly GenericController<TestEntity> _controller;

    public GenericControllerTests()
    {
        _mockUnitOfWork = new Mock<IGenericUnitOfWork<TestEntity>>();
        _controller = new GenericController<TestEntity>(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetAsync_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var expectedResponse = new ActionResponse<IEnumerable<TestEntity>>
        {
            WasSuccess = true,
            Result = new List<TestEntity> { new TestEntity { Id = 1, Name = "Test" } }
        };
        _mockUnitOfWork.Setup(x => x.GetAsync()).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<TestEntity>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task GetAsync_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        var expectedResponse = new ActionResponse<IEnumerable<TestEntity>>
        {
            WasSuccess = false,
            Message = "Error"
        };
        _mockUnitOfWork.Setup(x => x.GetAsync()).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAsync();

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var expectedResponse = new ActionResponse<TestEntity>
        {
            WasSuccess = true,
            Result = new TestEntity { Id = 1, Name = "Test" }
        };
        _mockUnitOfWork.Setup(x => x.GetAsync(1)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAsync(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<TestEntity>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsNotFound_WhenFailure()
    {
        // Arrange
        var expectedResponse = new ActionResponse<TestEntity>
        {
            WasSuccess = false,
            Message = "Not found"
        };
        _mockUnitOfWork.Setup(x => x.GetAsync(1)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAsync(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PostAsync_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var expectedResponse = new ActionResponse<TestEntity>
        {
            WasSuccess = true,
            Result = entity
        };
        _mockUnitOfWork.Setup(x => x.AddAsync(entity)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.PostAsync(entity);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<TestEntity>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task PostAsync_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var expectedResponse = new ActionResponse<TestEntity>
        {
            WasSuccess = false,
            Message = "Error"
        };
        _mockUnitOfWork.Setup(x => x.AddAsync(entity)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.PostAsync(entity);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error", badRequestResult.Value);
    }

    [Fact]
    public async Task PutAsync_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var expectedResponse = new ActionResponse<TestEntity>
        {
            WasSuccess = true,
            Result = entity
        };
        _mockUnitOfWork.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.PutAsync(entity);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<TestEntity>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task PutAsync_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var expectedResponse = new ActionResponse<TestEntity>
        {
            WasSuccess = false,
            Message = "Error"
        };
        _mockUnitOfWork.Setup(x => x.UpdateAsync(entity)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.PutAsync(entity);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error", badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContent_WhenSuccess()
    {
        // Arrange
        var expectedResponse = new ActionResponse<TestEntity>
        {
            WasSuccess = true
        };
        _mockUnitOfWork.Setup(x => x.DeleteAsync(1)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.DeleteAsync(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        var expectedResponse = new ActionResponse<TestEntity>
        {
            WasSuccess = false,
            Message = "Error"
        };
        _mockUnitOfWork.Setup(x => x.DeleteAsync(1)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.DeleteAsync(1);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error", badRequestResult.Value);
    }

    [Fact]
    public async Task GetAsync_WithPagination_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
        var expectedResponse = new ActionResponse<IEnumerable<TestEntity>>
        {
            WasSuccess = true,
            Result = new List<TestEntity> { new TestEntity { Id = 1, Name = "Test" } }
        };
        _mockUnitOfWork.Setup(x => x.GetAsync(pagination)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAsync(pagination);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<TestEntity>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task GetAsync_WithPagination_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
        var expectedResponse = new ActionResponse<IEnumerable<TestEntity>>
        {
            WasSuccess = false,
            Message = "Error"
        };
        _mockUnitOfWork.Setup(x => x.GetAsync(pagination)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAsync(pagination);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var expectedResponse = new ActionResponse<int>
        {
            WasSuccess = true,
            Result = 1
        };
        _mockUnitOfWork.Setup(x => x.GetTotalRecordsAsync()).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetTotalRecordsAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<int>(okResult.Value);
        Assert.Equal(1, returnValue);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        var expectedResponse = new ActionResponse<int>
        {
            WasSuccess = false,
            Message = "Error"
        };
        _mockUnitOfWork.Setup(x => x.GetTotalRecordsAsync()).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetTotalRecordsAsync();

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
} 