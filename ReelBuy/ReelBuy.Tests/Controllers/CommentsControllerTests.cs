using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReelBuy.Backend.Controllers;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;
using System.Security.Claims;

namespace ReelBuy.Tests.Controllers;

public class CommentsControllerTests
{
    private readonly Mock<IGenericUnitOfWork<Comments>> _mockGenericUnitOfWork;
    private readonly Mock<ICommentsUnitOfWork> _mockCommentsUnitOfWork;
    private readonly CommentsController _controller;

    public CommentsControllerTests()
    {
        _mockGenericUnitOfWork = new Mock<IGenericUnitOfWork<Comments>>();
        _mockCommentsUnitOfWork = new Mock<ICommentsUnitOfWork>();

        _controller = new CommentsController(_mockGenericUnitOfWork.Object, _mockCommentsUnitOfWork.Object);

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
    public async Task GetAsync_ReturnsOkResult_WhenCommentsExist()
    {
        // Arrange
        var comments = new List<Comments>
        {
            new Comments { Id = 1, UserId = "test-user", ProductId = 1, Description = "Test comment" }
        };
        _mockCommentsUnitOfWork.Setup(x => x.GetAsync())
            .ReturnsAsync(new ActionResponse<IEnumerable<Comments>> { WasSuccess = true, Result = comments });

        // Act
        var result = await _controller.GetAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Comments>>(okResult.Value);
        Assert.Single(returnValue);
        Assert.Equal("Test comment", returnValue[0].Description);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsOkResult_WhenCommentExists()
    {
        // Arrange
        var comment = new Comments { Id = 1, UserId = "test-user", ProductId = 1, Description = "Test comment" };
        _mockCommentsUnitOfWork.Setup(x => x.GetAsync(1))
            .ReturnsAsync(new ActionResponse<Comments> { WasSuccess = true, Result = comment });

        // Act
        var result = await _controller.GetAsync(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Comments>(okResult.Value);
        Assert.Equal("Test comment", returnValue.Description);
    }

    [Fact]
    public async Task GetAsync_WithPagination_ReturnsOkResult_WhenCommentsExist()
    {
        // Arrange
        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
        var comments = new List<Comments>
        {
            new Comments { Id = 1, UserId = "test-user", ProductId = 1, Description = "Test comment" }
        };
        _mockCommentsUnitOfWork.Setup(x => x.GetAsync(pagination))
            .ReturnsAsync(new ActionResponse<IEnumerable<Comments>> { WasSuccess = true, Result = comments });

        // Act
        var result = await _controller.GetAsync(pagination);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Comments>>(okResult.Value);
        Assert.Single(returnValue);
        Assert.Equal("Test comment", returnValue[0].Description);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ReturnsOkResult()
    {
        // Arrange
        _mockGenericUnitOfWork.Setup(x => x.GetTotalRecordsAsync())
            .ReturnsAsync(new ActionResponse<int> { WasSuccess = true, Result = 1 });

        // Act
        var result = await _controller.GetTotalRecordsAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<int>(okResult.Value);
        Assert.Equal(1, returnValue);
    }

    [Fact]
    public async Task GetCommentsByProductAsync_ReturnsOkResult_WhenCommentsExist()
    {
        // Arrange
        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10, Filter = "1" };
        var comments = new List<Comments>
        {
            new Comments { Id = 1, UserId = "test-user", ProductId = 1, Description = "Test comment" }
        };
        _mockCommentsUnitOfWork.Setup(x => x.GetCommentsByProductAsync(pagination))
            .ReturnsAsync(new ActionResponse<IEnumerable<Comments>> { WasSuccess = true, Result = comments });

        // Act
        var result = await _controller.GetCommentsByProductAsync(pagination);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Comments>>(okResult.Value);
        Assert.Single(returnValue);
        Assert.Equal("Test comment", returnValue[0].Description);
    }

    [Fact]
    public async Task PostAsync_ReturnsOkResult_WhenCommentIsAdded()
    {
        // Arrange
        var commentDTO = new CommetDTO
        {
            UserId = "test-user",
            ProductId = 1,
            Description = "Test comment",
            RegistrationDate = DateTime.UtcNow
        };
        var comment = new Comments
        {
            Id = 1,
            UserId = commentDTO.UserId,
            ProductId = commentDTO.ProductId,
            Description = commentDTO.Description,
            RegistrationDate = commentDTO.RegistrationDate
        };
        _mockCommentsUnitOfWork.Setup(x => x.AddAsync(commentDTO))
            .ReturnsAsync(new ActionResponse<Comments> { WasSuccess = true, Result = comment });

        // Act
        var result = await _controller.PostAsync(commentDTO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Comments>(okResult.Value);
        Assert.Equal(commentDTO.Description, returnValue.Description);
    }

    [Fact]
    public async Task PostAsync_ReturnsBadRequest_WhenCommentAdditionFails()
    {
        // Arrange
        var commentDTO = new CommetDTO
        {
            UserId = "test-user",
            ProductId = 1,
            Description = "Test comment",
            RegistrationDate = DateTime.UtcNow
        };
        _mockCommentsUnitOfWork.Setup(x => x.AddAsync(commentDTO))
            .ReturnsAsync(new ActionResponse<Comments> { WasSuccess = false, Message = "Error adding comment" });

        // Act
        var result = await _controller.PostAsync(commentDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error adding comment", badRequestResult.Value);
    }
} 