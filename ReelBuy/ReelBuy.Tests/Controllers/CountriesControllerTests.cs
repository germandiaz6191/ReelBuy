using Microsoft.AspNetCore.Mvc;
using Moq;
using ReelBuy.Backend.Controllers;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Tests.Controllers;

public class CountriesControllerTests
{
    private readonly Mock<IGenericUnitOfWork<Country>> _mockUnitOfWork;
    private readonly Mock<ICountriesUnitOfWork> _mockCountriesUnitOfWork;
    private readonly CountriesController _controller;

    public CountriesControllerTests()
    {
        _mockUnitOfWork = new Mock<IGenericUnitOfWork<Country>>();
        _mockCountriesUnitOfWork = new Mock<ICountriesUnitOfWork>();
        _controller = new CountriesController(_mockUnitOfWork.Object, _mockCountriesUnitOfWork.Object);
    }

    [Fact]
    public async Task GetComboAsync_ReturnsOkResult()
    {
        // Arrange
        var expectedList = new List<Country> { new Country { Id = 1, Name = "Test" } };
        _mockCountriesUnitOfWork.Setup(x => x.GetComboAsync()).ReturnsAsync(expectedList);

        // Act
        var result = await _controller.GetComboAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Country>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task GetAsync_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var expectedResponse = new ActionResponse<IEnumerable<Country>>
        {
            WasSuccess = true,
            Result = new List<Country> { new Country { Id = 1, Name = "Test" } }
        };
        _mockCountriesUnitOfWork.Setup(x => x.GetAsync()).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Country>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task GetAsync_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        var expectedResponse = new ActionResponse<IEnumerable<Country>>
        {
            WasSuccess = false,
            Message = "Error"
        };
        _mockCountriesUnitOfWork.Setup(x => x.GetAsync()).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAsync();

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var expectedResponse = new ActionResponse<Country>
        {
            WasSuccess = true,
            Result = new Country { Id = 1, Name = "Test" }
        };
        _mockCountriesUnitOfWork.Setup(x => x.GetAsync(1)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAsync(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Country>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsNotFound_WhenFailure()
    {
        // Arrange
        var expectedResponse = new ActionResponse<Country>
        {
            WasSuccess = false,
            Message = "Not found"
        };
        _mockCountriesUnitOfWork.Setup(x => x.GetAsync(1)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAsync(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetAsync_WithPagination_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
        var expectedResponse = new ActionResponse<IEnumerable<Country>>
        {
            WasSuccess = true,
            Result = new List<Country> { new Country { Id = 1, Name = "Test" } }
        };
        _mockCountriesUnitOfWork.Setup(x => x.GetAsync(pagination)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAsync(pagination);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Country>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task GetAsync_WithPagination_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
        var expectedResponse = new ActionResponse<IEnumerable<Country>>
        {
            WasSuccess = false,
            Message = "Error"
        };
        _mockCountriesUnitOfWork.Setup(x => x.GetAsync(pagination)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAsync(pagination);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
        var expectedResponse = new ActionResponse<int>
        {
            WasSuccess = true,
            Result = 1
        };
        _mockCountriesUnitOfWork.Setup(x => x.GetTotalRecordsAsync(pagination)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetTotalRecordsAsync(pagination);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<int>(okResult.Value);
        Assert.Equal(1, returnValue);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
        var expectedResponse = new ActionResponse<int>
        {
            WasSuccess = false,
            Message = "Error"
        };
        _mockCountriesUnitOfWork.Setup(x => x.GetTotalRecordsAsync(pagination)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetTotalRecordsAsync(pagination);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
} 