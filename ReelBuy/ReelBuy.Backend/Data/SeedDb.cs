using Microsoft.EntityFrameworkCore;

namespace ReelBuy.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;

    public SeedDb(DataContext context)
    {
        _context = context;
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
}