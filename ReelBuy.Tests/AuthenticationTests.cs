using Xunit;
using ReelBuy.Frontend.AuthenticationProviders;
using ReelBuy.Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ReelBuy.Tests;

public class AuthenticationTests
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var authProvider = new AuthenticationProviderJWT();
        var loginService = authProvider as ILoginService;

        // Act
        var result = await loginService!.LoginAsync("test@example.com", "password123");

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsFailure()
    {
        // Arrange
        var authProvider = new AuthenticationProviderJWT();
        var loginService = authProvider as ILoginService;

        // Act
        var result = await loginService!.LoginAsync("invalid@example.com", "wrongpassword");

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public async Task GetAuthenticationState_WhenLoggedIn_ReturnsAuthenticatedUser()
    {
        // Arrange
        var authProvider = new AuthenticationProviderJWT();
        var loginService = authProvider as ILoginService;
        await loginService!.LoginAsync("test@example.com", "password123");

        // Act
        var authState = await authProvider.GetAuthenticationStateAsync();

        // Assert
        Assert.True(authState.User.Identity!.IsAuthenticated);
        Assert.Equal("test@example.com", authState.User.FindFirst(ClaimTypes.Email)?.Value);
    }
} 