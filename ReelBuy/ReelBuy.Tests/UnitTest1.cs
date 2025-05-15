using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;

namespace ReelBuy.Tests
{
    public abstract class TestBase
    {
        protected DataContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new DataContext(options);
        }

        protected ClaimsPrincipal GetUserWithRole(string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser@example.com"),
                new Claim(ClaimTypes.Email, "testuser@example.com"),
                new Claim(ClaimTypes.NameIdentifier, "testuser"),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "Test");
            return new ClaimsPrincipal(identity);
        }

        protected ClaimsPrincipal GetUserWithoutRole()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser@example.com"),
                new Claim(ClaimTypes.Email, "testuser@example.com"),
                new Claim(ClaimTypes.NameIdentifier, "testuser")
            };

            var identity = new ClaimsIdentity(claims, "Test");
            return new ClaimsPrincipal(identity);
        }
    }
}