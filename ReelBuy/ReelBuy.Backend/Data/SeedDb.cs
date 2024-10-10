using Microsoft.EntityFrameworkCore;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Enums;
using ReelBuy.Backend.UnitsOfWork.Interfaces;

namespace ReelBuy.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;
    private readonly IUsersUnitOfWork _usersUnitOfWork;

    public SeedDb(DataContext context, IUsersUnitOfWork usersUnitOfWork)
    {
        _context = context;
        _usersUnitOfWork = usersUnitOfWork;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckCountriesAsync();
        await CheckCategoriesAsync();
        await CheckProfilesAsync();
        await CheckMarketplacesAsync();
        await CheckReputationsAsync();
        await CheckStatusesAsync();
        await CheckRolesAsync();
        await CheckUserAsync("Reel", "Buy", "reelbuy@yopmail.com", "310 520 3206", UserType.Admin);
    }

    private async Task CheckCountriesAsync()
    {
        if (!_context.Countries.Any())
        {
            var countriesSQLScript = File.ReadAllText("Data\\Countries.sql");
            await _context.Database.ExecuteSqlRawAsync(countriesSQLScript);
        }
    }

    private async Task CheckProfilesAsync()
    {
        if (!_context.Profiles.Any())
        {
            var profilesSQLScript = File.ReadAllText("Data\\Profiles.sql");
            await _context.Database.ExecuteSqlRawAsync(profilesSQLScript);
        }
    }

    private async Task CheckCategoriesAsync()
    {
        if (!_context.Categories.Any())
        {
            var categoriesSQLScript = File.ReadAllText("Data\\Categories.sql");
            await _context.Database.ExecuteSqlRawAsync(categoriesSQLScript);
        }
    }

    private async Task CheckMarketplacesAsync()
    {
        if (!_context.Marketplaces.Any())
        {
            var marketplacesSQLScript = File.ReadAllText("Data\\Marketplaces.sql");
            await _context.Database.ExecuteSqlRawAsync(marketplacesSQLScript);
        }
    }

    private async Task CheckStatusesAsync()
    {
        if (!_context.Statuses.Any())
        {
            var statusesSQLScript = File.ReadAllText("Data\\Statuses.sql");
            await _context.Database.ExecuteSqlRawAsync(statusesSQLScript);
        }
    }

    private async Task CheckReputationsAsync()
    {
        if (!_context.Reputations.Any())
        {
            var reputationsSQLScript = File.ReadAllText("Data\\Reputations.sql");
            await _context.Database.ExecuteSqlRawAsync(reputationsSQLScript);
        }
    }

    private async Task CheckRolesAsync()
    {
        await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
        await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
    }

    private async Task<User> CheckUserAsync(string firstName, string lastName, string email, string phone, UserType userType)
    {
        var user = await _usersUnitOfWork.GetUserAsync(email);
        if (user == null)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(x => x.Name == "Colombia");
            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                PhoneNumber = phone,
                Country = country!,
                UserType = userType,
            };

            await _usersUnitOfWork.AddUserAsync(user, "123456");
            await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());

            var token = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
            await _usersUnitOfWork.ConfirmEmailAsync(user, token);
        }

        return user;
    }
}