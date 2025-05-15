using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReelBuy.Backend.Controllers;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ReelBuy.Tests.Controllers;

public class FavoritesControllerTests
{
    private readonly Mock<IGenericUnitOfWork<Favorite>> _mockGenericUnitOfWork;
    private readonly Mock<IFavoritesUnitOfWork> _mockFavoritesUnitOfWork;
    private readonly FavoritesController _controller;

    public FavoritesControllerTests()
    {
        _mockGenericUnitOfWork = new Mock<IGenericUnitOfWork<Favorite>>();
        _mockFavoritesUnitOfWork = new Mock<IFavoritesUnitOfWork>();

        _controller = new FavoritesController(_mockGenericUnitOfWork.Object, _mockFavoritesUnitOfWork.Object);

        // Setup default authorization
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "test@example.com"),
            new Claim(ClaimTypes.Role, "User")
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetComboAsync_ReturnsOkResult()
    {
        // Arrange
        var combo = new List<Favorite>();
        _mockFavoritesUnitOfWork.Setup(x => x.GetComboAsync())
            .ReturnsAsync(combo);

        // Act
        var result = await _controller.GetComboAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Favorite>>(okResult.Value);
        Assert.Equal(combo, returnValue);
    }

    [Fact]
    public async Task GetAsync_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var favorites = new List<Favorite>();
        _mockFavoritesUnitOfWork.Setup(x => x.GetAsync())
            .ReturnsAsync(new ActionResponse<IEnumerable<Favorite>> { WasSuccess = true, Result = favorites });

        // Act
        var result = await _controller.GetAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Favorite>>(okResult.Value);
        Assert.Equal(favorites, returnValue);
    }

    [Fact]
    public async Task GetAsync_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        _mockFavoritesUnitOfWork.Setup(x => x.GetAsync())
            .ReturnsAsync(new ActionResponse<IEnumerable<Favorite>> { WasSuccess = false });

        // Act
        var result = await _controller.GetAsync();

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var favorite = new Favorite { Id = 1 };
        _mockFavoritesUnitOfWork.Setup(x => x.GetAsync(1))
            .ReturnsAsync(new ActionResponse<Favorite> { WasSuccess = true, Result = favorite });

        // Act
        var result = await _controller.GetAsync(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Favorite>(okResult.Value);
        Assert.Equal(favorite, returnValue);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsNotFound_WhenFailure()
    {
        // Arrange
        _mockFavoritesUnitOfWork.Setup(x => x.GetAsync(1))
            .ReturnsAsync(new ActionResponse<Favorite> { WasSuccess = false, Message = "Not found" });

        // Act
        var result = await _controller.GetAsync(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetFavoriteAsync_ReturnsOkResult_WhenFavoriteExists()
    {
        // Arrange
        var userId = "test-user";
        var productId = 1;
        var favorite = new Favorite { Id = 1, UserId = userId, ProductId = productId };
        _mockFavoritesUnitOfWork.Setup(x => x.GetAsync(userId, productId))
            .ReturnsAsync(new ActionResponse<Favorite> { WasSuccess = true, Result = favorite });

        // Act
        var result = await _controller.GetFavoriteAsync(userId, productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Favorite>(okResult.Value);
        Assert.Equal(favorite, returnValue);
    }

    [Fact]
    public async Task GetFavoriteAsync_ReturnsBadRequest_WhenFavoriteDoesNotExist()
    {
        // Arrange
        var userId = "test-user";
        var productId = 1;
        _mockFavoritesUnitOfWork.Setup(x => x.GetAsync(userId, productId))
            .ReturnsAsync(new ActionResponse<Favorite> { WasSuccess = false });

        // Act
        var result = await _controller.GetFavoriteAsync(userId, productId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetAsync_WithPagination_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
        var favorites = new List<Favorite>();
        _mockFavoritesUnitOfWork.Setup(x => x.GetAsync(pagination))
            .ReturnsAsync(new ActionResponse<IEnumerable<Favorite>> { WasSuccess = true, Result = favorites });

        // Act
        var result = await _controller.GetAsync(pagination);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Favorite>>(okResult.Value);
        Assert.Equal(favorites, returnValue);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
        _mockFavoritesUnitOfWork.Setup(x => x.GetTotalRecordsAsync(pagination))
            .ReturnsAsync(new ActionResponse<int> { WasSuccess = true, Result = 5 });

        // Act
        var result = await _controller.GetTotalRecordsAsync(pagination);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<int>(okResult.Value);
        Assert.Equal(5, returnValue);
    }

    [Fact]
    public async Task PostAsync_ReturnsOkResult_WhenSuccess()
    {
        // Arrange
        var favoriteDTO = new FavoriteDTO { UserId = "test-user", ProductId = 1 };
        var favorite = new Favorite { Id = 1, UserId = favoriteDTO.UserId, ProductId = favoriteDTO.ProductId };
        _mockFavoritesUnitOfWork.Setup(x => x.AddAsync(favoriteDTO))
            .ReturnsAsync(new ActionResponse<Favorite> { WasSuccess = true, Result = favorite });

        // Act
        var result = await _controller.PostAsync(favoriteDTO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Favorite>(okResult.Value);
        Assert.Equal(favorite, returnValue);
    }

    [Fact]
    public async Task PostAsync_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        var favoriteDTO = new FavoriteDTO { UserId = "test-user", ProductId = 1 };
        _mockFavoritesUnitOfWork.Setup(x => x.AddAsync(favoriteDTO))
            .ReturnsAsync(new ActionResponse<Favorite> { WasSuccess = false, Message = "Error adding favorite" });

        // Act
        var result = await _controller.PostAsync(favoriteDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error adding favorite", badRequestResult.Value);
    }
} 