using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Moq;
using ReelBuy.Backend.Controllers;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;
using System.Security.Claims;

namespace ReelBuy.Tests.Controllers;

public class LikesControllerTests
{
    private readonly Mock<ILikesUnitOfWork> _mockLikesUnitOfWork;
    private readonly LikesController _controller;

    public LikesControllerTests()
    {
        _mockLikesUnitOfWork = new Mock<ILikesUnitOfWork>();

        _controller = new LikesController(_mockLikesUnitOfWork.Object);

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
    public async Task GetLikeAsync_ReturnsOkResult_WhenLikeExists()
    {
        // Arrange
        var userId = "test-user";
        var productId = 1;
        _mockLikesUnitOfWork.Setup(x => x.GetAsync(userId, productId))
            .ReturnsAsync(new ActionResponse<bool> { WasSuccess = true, Result = true });

        // Act
        var result = await _controller.GetLikeAsync(userId, productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<bool>(okResult.Value);
        Assert.True(returnValue);
    }

    [Fact]
    public async Task GetLikeAsync_ReturnsNotFound_WhenLikeDoesNotExist()
    {
        // Arrange
        var userId = "test-user";
        var productId = 1;
        _mockLikesUnitOfWork.Setup(x => x.GetAsync(userId, productId))
            .ReturnsAsync(new ActionResponse<bool> { WasSuccess = false, Message = "Like not found" });

        // Act
        var result = await _controller.GetLikeAsync(userId, productId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Like not found", notFoundResult.Value);
    }

    [Fact]
    public async Task PostAsync_ReturnsOkResult_WhenLikeIsAdded()
    {
        // Arrange
        var likeDTO = new LikeDTO { UserId = "test-user", ProductId = 1 };
        _mockLikesUnitOfWork.Setup(x => x.AddAsync(likeDTO))
            .ReturnsAsync(new ActionResponse<int> { WasSuccess = true, Result = 1 });

        // Act
        var result = await _controller.PostAsync(likeDTO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<int>(okResult.Value);
        Assert.Equal(1, returnValue);
    }

    [Fact]
    public async Task PostAsync_ReturnsBadRequest_WhenLikeAdditionFails()
    {
        // Arrange
        var likeDTO = new LikeDTO { UserId = "test-user", ProductId = 1 };
        _mockLikesUnitOfWork.Setup(x => x.AddAsync(likeDTO))
            .ReturnsAsync(new ActionResponse<int> { WasSuccess = false, Message = "Error adding like" });

        // Act
        var result = await _controller.PostAsync(likeDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error adding like", badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteLikeAsync_ReturnsOkResult_WhenLikeIsDeleted()
    {
        // Arrange
        var userId = "test-user";
        var productId = 1;
        _mockLikesUnitOfWork.Setup(x => x.DeleteAsync(userId, productId))
            .ReturnsAsync(new ActionResponse<int> { WasSuccess = true, Result = 1 });

        // Act
        var result = await _controller.DeleteLikeAsync(userId, productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<int>(okResult.Value);
        Assert.Equal(1, returnValue);
    }

    [Fact]
    public async Task DeleteLikeAsync_ReturnsBadRequest_WhenLikeDeletionFails()
    {
        // Arrange
        var userId = "test-user";
        var productId = 1;
        _mockLikesUnitOfWork.Setup(x => x.DeleteAsync(userId, productId))
            .ReturnsAsync(new ActionResponse<int> { WasSuccess = false, Message = "Error deleting like" });

        // Act
        var result = await _controller.DeleteLikeAsync(userId, productId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error deleting like", badRequestResult.Value);
    }
} 