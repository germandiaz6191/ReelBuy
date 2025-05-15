using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace ReelBuy.Tests.Repositories;

public class UsersRepositoryTests : IDisposable
{
    private readonly DataContext _context;
    private readonly UsersRepository _repository;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;

    public UsersRepositoryTests()
    {
        var services = new ServiceCollection();

        // Add configuration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Add logging
        services.AddSingleton<ILogger<UserManager<User>>>(new NullLogger<UserManager<User>>());
        services.AddSingleton<ILogger<RoleManager<IdentityRole>>>(new NullLogger<RoleManager<IdentityRole>>());
        services.AddSingleton<ILogger<SignInManager<User>>>(new NullLogger<SignInManager<User>>());
        services.AddSingleton<ILogger<DataProtectorTokenProvider<User>>>(new NullLogger<DataProtectorTokenProvider<User>>());

        // Add DbContext
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new DataContext(options);

        // Add Data Protection
        services.AddDataProtection();

        // Add Identity
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);
        var contextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
        _signInManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object, contextAccessorMock.Object, userClaimsPrincipalFactoryMock.Object, null, null, null, null);
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
        })
        .AddEntityFrameworkStores<DataContext>()
        .AddDefaultTokenProviders();

        var serviceProvider = services.BuildServiceProvider();

        // Get required services
        _repository = new UsersRepository(_context, _userManagerMock.Object, _roleManagerMock.Object, _signInManagerMock.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddUserAsync_ShouldReturnSuccess()
    {
        var user = new User { Email = "test@example.com" };
        _userManagerMock.Setup(x => x.CreateAsync(user, "Password123!")).ReturnsAsync(IdentityResult.Success);
        var result = await _repository.AddUserAsync(user, "Password123!");
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task GetUserAsync_ByEmail_ShouldReturnUser()
    {
        var country = new Country { Id = 1, Name = "TestCountry" };
        var profile = new Profile { Id = 1, Name = "TestProfile" };
        _context.Countries.Add(country);
        _context.Profiles.Add(profile);
        _context.SaveChanges();
        var user = new User {
            Id = "1",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            CountryId = country.Id,
            Country = country,
            ProfileId = profile.Id,
            Profile = profile
        };
        _context.Users.Add(user);
        _context.SaveChanges();
        var result = await _repository.GetUserAsync("test@example.com");
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetUserAsync_ByGuid_ShouldReturnUser()
    {
        var country = new Country { Id = 2, Name = "TestCountry2" };
        var profile = new Profile { Id = 2, Name = "TestProfile2" };
        _context.Countries.Add(country);
        _context.Profiles.Add(profile);
        _context.SaveChanges();
        var user = new User {
            Id = Guid.NewGuid().ToString(),
            Email = "test2@example.com",
            FirstName = "Test",
            LastName = "User",
            CountryId = country.Id,
            Country = country,
            ProfileId = profile.Id,
            Profile = profile
        };
        _context.Users.Add(user);
        _context.SaveChanges();
        var result = await _repository.GetUserAsync(Guid.Parse(user.Id));
        Assert.NotNull(result);
        Assert.Equal("test2@example.com", result.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess()
    {
        var loginDto = new LoginDTO { Email = "test@example.com", Password = "Password123!" };
        _signInManagerMock.Setup(x => x.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false)).ReturnsAsync(SignInResult.Success);
        var result = await _repository.LoginAsync(loginDto);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnSuccess()
    {
        var user = new User { Email = "test@example.com" };
        _userManagerMock.Setup(x => x.ChangePasswordAsync(user, "oldPass", "newPass")).ReturnsAsync(IdentityResult.Success);
        var result = await _repository.ChangePasswordAsync(user, "oldPass", "newPass");
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldReturnSuccess()
    {
        var user = new User { Email = "test@example.com" };
        _userManagerMock.Setup(x => x.ResetPasswordAsync(user, "token", "newPass")).ReturnsAsync(IdentityResult.Success);
        var result = await _repository.ResetPasswordAsync(user, "token", "newPass");
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task ConfirmEmailAsync_ShouldReturnSuccess()
    {
        var user = new User { Email = "test@example.com" };
        _userManagerMock.Setup(x => x.ConfirmEmailAsync(user, "token")).ReturnsAsync(IdentityResult.Success);
        var result = await _repository.ConfirmEmailAsync(user, "token");
        Assert.True(result.Succeeded);
    }
} 