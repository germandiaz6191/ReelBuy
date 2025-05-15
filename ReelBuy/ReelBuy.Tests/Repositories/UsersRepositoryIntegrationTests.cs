using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Shared.Entities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ReelBuy.Tests.Repositories
{
    public class UsersRepositoryIntegrationTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly UsersRepository _repository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ServiceProvider _serviceProvider;

        public UsersRepositoryIntegrationTests()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<DataContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();
            _serviceProvider = services.BuildServiceProvider();
            _context = _serviceProvider.GetRequiredService<DataContext>();
            _userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
            _roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _signInManager = _serviceProvider.GetRequiredService<SignInManager<User>>();
            _repository = new UsersRepository(_context, _userManager, _roleManager, _signInManager);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _serviceProvider.Dispose();
        }

        [Fact]
        public async Task AddUserAsync_And_GetUserAsync_ShouldWorkWithRealDb()
        {
            // Arrange
            var user = new User
            {
                Email = "integration@example.com",
                UserName = "integration@example.com",
                FirstName = "Integration",
                LastName = "Test"
            };
            var password = "Password123!";

            // Act
            var addResult = await _repository.AddUserAsync(user, password);
            var retrievedUser = await _repository.GetUserAsync("integration@example.com");

            // Assert
            Assert.True(addResult.Succeeded);
            Assert.NotNull(retrievedUser);
            Assert.Equal("integration@example.com", retrievedUser.Email);
        }
    }
} 