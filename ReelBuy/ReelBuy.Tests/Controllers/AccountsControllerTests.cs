using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using ReelBuy.Backend.Controllers;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ReelBuy.Tests.Controllers;

public class AccountsControllerTests
{
    private readonly Mock<IUsersUnitOfWork> _mockUsersUnitOfWork;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IMailHelper> _mockMailHelper;
    private readonly Mock<IFileStorage> _mockFileStorage;
    private readonly Mock<IImageResizer> _mockImageResizer;
    private readonly AccountsController _controller;

    public AccountsControllerTests()
    {
        _mockUsersUnitOfWork = new Mock<IUsersUnitOfWork>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockMailHelper = new Mock<IMailHelper>();
        _mockFileStorage = new Mock<IFileStorage>();
        _mockImageResizer = new Mock<IImageResizer>();

        // Setup configuration
        _mockConfiguration.Setup(x => x["JWT:Key"]).Returns("your-very-very-very-very-secret-key!!");
        _mockConfiguration.Setup(x => x["JWT:Issuer"]).Returns("your-issuer");
        _mockConfiguration.Setup(x => x["JWT:Audience"]).Returns("your-audience");
        _mockConfiguration.Setup(x => x["Mail:From"]).Returns("test@example.com");
        _mockConfiguration.Setup(x => x["Mail:Name"]).Returns("Test Name");
        _mockConfiguration.Setup(x => x["Mail:Smtp"]).Returns("smtp.example.com");
        _mockConfiguration.Setup(x => x["Mail:Port"]).Returns("587");
        _mockConfiguration.Setup(x => x["Mail:Password"]).Returns("password");
        _mockConfiguration.Setup(x => x["jwtKey"]).Returns("your-very-very-very-very-secret-key!!");
        _mockConfiguration.Setup(x => x["Url Frontend"]).Returns("http://localhost:5000");
        _mockConfiguration.Setup(x => x["Mail:SubjectConfirmationEs"]).Returns("Confirmación de correo");
        _mockConfiguration.Setup(x => x["Mail:BodyConfirmationEs"]).Returns("Confirma tu correo aquí: {0}");
        _mockConfiguration.Setup(x => x["Mail:SubjectConfirmationEn"]).Returns("Email confirmation");
        _mockConfiguration.Setup(x => x["Mail:BodyConfirmationEn"]).Returns("Confirm your email here: {0}");
        _mockConfiguration.Setup(x => x["Mail:SubjectRecoveryEs"]).Returns("Recuperación de contraseña");
        _mockConfiguration.Setup(x => x["Mail:BodyRecoveryEs"]).Returns("Recupera tu contraseña aquí: {0}");
        _mockConfiguration.Setup(x => x["Mail:SubjectRecoveryEn"]).Returns("Password recovery");
        _mockConfiguration.Setup(x => x["Mail:BodyRecoveryEn"]).Returns("Reset your password here: {0}");

        _controller = new AccountsController(
            _mockUsersUnitOfWork.Object,
            _mockConfiguration.Object,
            _mockMailHelper.Object,
            new DataContext(new DbContextOptionsBuilder<DataContext>().Options),
            _mockFileStorage.Object,
            _mockImageResizer.Object
        );

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

        // Setup UrlHelper
        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(x => x.Link(
            It.IsAny<string>(),
            It.IsAny<object>()
        )).Returns("http://localhost:5000/confirm-email");
        _controller.Url = urlHelper.Object;
    }

    private AccountsController CreateControllerWithContext(DataContext? context = null)
    {
        var controller = new AccountsController(
            _mockUsersUnitOfWork.Object,
            _mockConfiguration.Object,
            _mockMailHelper.Object,
            context ?? new DataContext(new DbContextOptionsBuilder<DataContext>().Options),
            _mockFileStorage.Object,
            _mockImageResizer.Object
        );
        controller.ControllerContext = _controller.ControllerContext;
        controller.Url = _controller.Url;
        return controller;
    }

    [Fact]
    public void HandlePreflight_ReturnsOkResult()
    {
        // Act
        var result = _controller.HandlePreflight();

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task CreateUser_ReturnsOkResult_WhenUserIsCreated()
    {
        // Arrange
        var userDTO = new UserDTO
        {
            Email = "test@example.com",
            Password = "Test123!",
            FirstName = "Test",
            LastName = "User",
            CountryId = 1,
            ProfileId = 1
        };
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "CreateUserOkTest")
            .Options;
        using var context = new DataContext(options);
        context.Countries.Add(new Country { Id = 1, Name = "Test Country" });
        context.Profiles.Add(new Profile { Id = 1, Name = "Test Profile" });
        context.SaveChanges();
        var controller = CreateControllerWithContext(context);
        _mockUsersUnitOfWork.Setup(x => x.AddUserAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _mockUsersUnitOfWork.Setup(x => x.AddUserToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _mockMailHelper.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new ActionResponse<string> { WasSuccess = true });

        // Act
        var result = await controller.CreateUser(userDTO);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenUserCreationFails()
    {
        // Arrange
        var userDTO = new UserDTO
        {
            Email = "test@example.com",
            Password = "Test123!",
            FirstName = "Test",
            LastName = "User",
            CountryId = 1,
            ProfileId = 1
        };
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "CreateUserBadRequestTest")
            .Options;
        using var context = new DataContext(options);
        context.Countries.Add(new Country { Id = 1, Name = "Test Country" });
        context.Profiles.Add(new Profile { Id = 1, Name = "Test Profile" });
        context.SaveChanges();
        var controller = CreateControllerWithContext(context);
        var error = new IdentityError { Description = "Error creating user" };
        _mockUsersUnitOfWork.Setup(x => x.AddUserAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(error));

        // Act
        var result = await controller.CreateUser(userDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnedError = Assert.IsType<IdentityError>(badRequestResult.Value);
        Assert.Equal(error.Description, returnedError.Description);
    }

    [Fact]
    public async Task ConfirmEmailAsync_ReturnsOkResult_WhenEmailIsConfirmed()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var user = new User { Id = userId, Email = "test@example.com" };
        var token = "test-token";
        _mockUsersUnitOfWork.Setup(x => x.GetUserAsync(It.IsAny<Guid>())).ReturnsAsync(user);
        _mockUsersUnitOfWork.Setup(x => x.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.ConfirmEmailAsync(userId, token);

        // Assert
        var okResult = Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task ConfirmEmailAsync_ReturnsBadRequest_WhenEmailConfirmationFails()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var user = new User { Id = userId, Email = "test@example.com" };
        var token = "test-token";
        var error = new IdentityError { Description = "Error confirming email" };
        _mockUsersUnitOfWork.Setup(x => x.GetUserAsync(It.IsAny<Guid>())).ReturnsAsync(user);
        _mockUsersUnitOfWork.Setup(x => x.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Failed(error));

        // Act
        var result = await _controller.ConfirmEmailAsync(userId, token);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnedError = Assert.IsType<IdentityError>(badRequestResult.Value);
        Assert.Equal(error.Description, returnedError.Description);
    }

    [Fact]
    public async Task LoginAsync_ReturnsOkResult_WhenLoginIsSuccessful()
    {
        // Arrange
        var loginDTO = new LoginDTO { Email = "test@example.com", Password = "Test123!" };
        var country = new Country { Id = 1, Name = "Test Country" };
        var profile = new Profile { Id = 1, Name = "Test Profile" };
        var user = new User 
        { 
            Id = "test-user", 
            Email = loginDTO.Email,
            FirstName = "Test",
            LastName = "User",
            Country = country,
            Profile = profile
        };

        _mockUsersUnitOfWork.Setup(x => x.LoginAsync(loginDTO)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        _mockUsersUnitOfWork.Setup(x => x.GetUserAsync(loginDTO.Email)).ReturnsAsync(user);

        // Act
        var result = await _controller.LoginAsync(loginDTO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<TokenDTO>(okResult.Value);
        Assert.NotNull(returnValue.Token);
        Assert.True(returnValue.Expiration > DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_ReturnsBadRequest_WhenLoginFails()
    {
        // Arrange
        var loginDTO = new LoginDTO { Email = "test@example.com", Password = "Test123!" };
        _mockUsersUnitOfWork.Setup(x => x.LoginAsync(loginDTO)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = await _controller.LoginAsync(loginDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ERR006", badRequestResult.Value);
    }

    [Fact]
    public async Task PutAsync_ReturnsOkResult_WhenUserIsUpdated()
    {
        // Arrange
        var country = new Country { Id = 1, Name = "Test Country" };
        var profile = new Profile { Id = 1, Name = "Customer" };
        var currentUser = new User 
        { 
            Id = "test-user", 
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Country = country,
            CountryId = country.Id,
            Profile = profile,
            ProfileId = profile.Id
        };
        _mockUsersUnitOfWork.Setup(x => x.GetUserAsync(It.IsAny<string>())).ReturnsAsync(currentUser);
        _mockUsersUnitOfWork.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
        _mockFileStorage.Setup(x => x.GetFileAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new byte[0]);
        _mockImageResizer.Setup(x => x.ResizeImage(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(new byte[0]);

        // Act
        var result = await _controller.PutAsync(currentUser);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var tokenDto = Assert.IsType<TokenDTO>(okResult.Value);
        Assert.NotNull(tokenDto.Token);
        Assert.True(tokenDto.Expiration > DateTime.UtcNow);
    }

    [Fact]
    public async Task PutAsync_ReturnsBadRequest_WhenUserUpdateFails()
    {
        // Arrange
        var userDTO = new UserDTO { Id = "test-user", Email = "test@example.com" };
        var currentUser = new User { Id = "test-user", Email = "test@example.com" };
        var error = new IdentityError { Description = "Error updating user" };
        _mockUsersUnitOfWork.Setup(x => x.GetUserAsync(It.IsAny<string>())).ReturnsAsync(currentUser);
        _mockUsersUnitOfWork.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Failed(error));

        // Act
        var result = await _controller.PutAsync(currentUser);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var returnedError = Assert.IsType<IdentityError>(badRequestResult.Value);
        Assert.Equal(error.Description, returnedError.Description);
    }

    [Fact]
    public async Task ChangePasswordAsync_ReturnsOkResult_WhenPasswordIsChanged()
    {
        // Arrange
        var changePasswordDTO = new ChangePasswordDTO { CurrentPassword = "OldPass123!", NewPassword = "NewPass123!" };
        var user = new User { Id = "test-user", Email = "test@example.com" };
        _mockUsersUnitOfWork.Setup(x => x.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
        _mockUsersUnitOfWork.Setup(x => x.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.ChangePasswordAsync(changePasswordDTO);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task ChangePasswordAsync_ReturnsBadRequest_WhenPasswordChangeFails()
    {
        // Arrange
        var changePasswordDTO = new ChangePasswordDTO { CurrentPassword = "OldPass123!", NewPassword = "NewPass123!" };
        var user = new User { Id = "test-user", Email = "test@example.com" };
        _mockUsersUnitOfWork.Setup(x => x.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
        _mockUsersUnitOfWork.Setup(x => x.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error changing password" }));

        // Act
        var result = await _controller.ChangePasswordAsync(changePasswordDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error changing password", badRequestResult.Value);
    }

    [Fact]
    public async Task RecoverPasswordAsync_ReturnsOkResult_WhenPasswordRecoveryIsSuccessful()
    {
        // Arrange
        var recoverPasswordDTO = new EmailDTO { Email = "test@example.com", Language = "es" };
        var user = new User { Id = "test-user", Email = recoverPasswordDTO.Email };
        _mockUsersUnitOfWork.Setup(x => x.GetUserAsync(recoverPasswordDTO.Email)).ReturnsAsync(user);
        _mockUsersUnitOfWork.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("reset-token");
        _mockMailHelper.Setup(x => x.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new ActionResponse<string> { WasSuccess = true });

        // Act
        var result = await _controller.RecoverPasswordAsync(recoverPasswordDTO);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task RecoverPasswordAsync_ReturnsBadRequest_WhenUserNotFound()
    {
        // Arrange
        var recoverPasswordDTO = new EmailDTO { Email = "test@example.com", Language = "es" };
        _mockUsersUnitOfWork.Setup(x => x.GetUserAsync(recoverPasswordDTO.Email)).ReturnsAsync((User)null!);

        // Act
        var result = await _controller.RecoverPasswordAsync(recoverPasswordDTO);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ResetPasswordAsync_ReturnsOkResult_WhenPasswordIsReset()
    {
        // Arrange
        var resetPasswordDTO = new ResetPasswordDTO { Email = "test@example.com", Token = "test-token", NewPassword = "NewPass123!", ConfirmPassword = "NewPass123!" };
        var user = new User { Id = "test-user", Email = resetPasswordDTO.Email };
        _mockUsersUnitOfWork.Setup(x => x.GetUserAsync(resetPasswordDTO.Email)).ReturnsAsync(user);
        _mockUsersUnitOfWork.Setup(x => x.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.ResetPasswordAsync(resetPasswordDTO);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task ResetPasswordAsync_ReturnsBadRequest_WhenPasswordResetFails()
    {
        // Arrange
        var resetPasswordDTO = new ResetPasswordDTO { Email = "test@example.com", Token = "test-token", NewPassword = "NewPass123!", ConfirmPassword = "NewPass123!" };
        var user = new User { Id = "test-user", Email = resetPasswordDTO.Email };
        _mockUsersUnitOfWork.Setup(x => x.GetUserAsync(resetPasswordDTO.Email)).ReturnsAsync(user);
        _mockUsersUnitOfWork.Setup(x => x.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword)).ReturnsAsync(Microsoft.AspNetCore.Identity.IdentityResult.Failed(new IdentityError { Description = "Error resetting password" }));

        // Act
        var result = await _controller.ResetPasswordAsync(resetPasswordDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error resetting password", badRequestResult.Value);
    }
} 