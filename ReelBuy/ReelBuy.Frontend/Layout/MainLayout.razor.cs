using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Layout;

public partial class MainLayout
{
    private bool _drawerOpen = true;
    private string _icon = Icons.Material.Filled.DarkMode;
    private string? selectedText;
    private List<string> searchResults = new List<string>();
    private const string baseUrl = "api/products/search";

    private bool _darkMode { get; set; } = true;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    private MudTheme CustomTheme => new MudTheme
    {
        Palette = new PaletteLight
        {
            Primary = "rgb(74, 81, 94)",
            Secondary = "rgb(63, 81, 181)",
            Background = "rgb(245, 245, 245)",
            Surface = "rgb(255, 255, 255)",
            AppbarBackground = "rgb(74, 81, 94)",
            AppbarText = "rgb(255, 255, 255)",
            TextPrimary = "rgb(33, 33, 33)",
            TextSecondary = "rgb(169, 169, 169)",
            DrawerBackground = "rgb(238, 238, 238)",
            DrawerText = "rgb(33, 33, 33)"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "rgb(74, 81, 94)",
            Secondary = "rgb(255, 159, 64)",
            Background = "rgb(18, 18, 18)",
            Surface = "rgb(51, 51, 51)",
            AppbarBackground = "rgb(74, 81, 94)",
            AppbarText = "rgb(255, 255, 255)",
            TextPrimary = "rgb(255, 255, 255)",
            TextSecondary = "rgb(169, 169, 169)",
            DrawerBackground = "rgb(30, 30, 30)",
            DrawerText = "rgb(255, 255, 255)"
        }
    };

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private void DarkModeToggle()
    {
        _darkMode = !_darkMode;
        _icon = _darkMode ? Icons.Material.Filled.LightMode : Icons.Material.Filled.DarkMode;
    }

    private async Task<IEnumerable<string>> SearchProduct(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText) || searchText.Length <= 3)
        {
            return new List<string>();
        }

        return await PerformSearch(searchText);
    }

    private async Task<List<string>> PerformSearch(string query)
    {
        var allResults = await GetSearchResults(query);

        searchResults = allResults;

        if (allResults.Count == 0)
        {
            searchResults.Add(Localizer["PrincipalSearchNotFoundResults"]);
        }
        if (allResults.Count > 10)
        {
            searchResults = allResults.Take(10).ToList();
            searchResults.Add(Localizer["PrincipalSearchExceededMaxResults"]);
        }

        return searchResults;
    }

    private async Task<List<string>> GetSearchResults(string query)
    {
        var principalSearchDTO = new PrincipalSearchDTO()
        {
            keyword = query
        };

        var url = $"{baseUrl}/?keyword={query}";

        var responseHttp = await Repository.GetAsync<List<Product>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return new List<string>();
        }

        return responseHttp.Response?.Select(product => product.Name).ToList() ?? new List<string>();
    }
}