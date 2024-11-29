using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Layout;

public partial class MainLayout
{
    private bool _drawerOpen = true;
    private string _icon = Icons.Material.Filled.DarkMode;
    private string? selectedText;
    private List<string> searchResults = new List<string>();

    private bool _darkMode { get; set; } = true;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    private MudTheme CustomTheme => new MudTheme
    {
        PaletteLight = new PaletteLight
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

    private async Task<IEnumerable<String>> SearchProduct(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);
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
        // Simula un retraso (esto se debe reemplazar con una búsqueda real)
        await Task.Delay(500);

        // Lista simulada de productos
        var allResults = new List<string>
        {
            "Producto 1", "Producto 2", "Producto 3", "Producto 4", "Producto 5",
            "Producto 6", "Producto 7", "Producto 8", "Producto 9", "Producto 10",
            "Producto 11", "Producto 12"
        };

        // Filtra los resultados que contienen el texto de búsqueda
        return allResults.Where(p => p.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}