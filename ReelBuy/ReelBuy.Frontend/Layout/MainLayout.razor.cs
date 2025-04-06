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
            Primary = "rgb(34, 150, 243)",        // Azul vibrante para dar dinamismo
            Secondary = "rgb(255, 87, 34)",         // Naranja fresco para contrastar
            Background = "rgb(250, 250, 250)",      // Fondo muy claro para destacar elementos
            Surface = "rgb(255, 255, 255)",         // Superficie limpia y minimalista
            AppbarBackground = "rgb(34, 150, 243)", // Consistente con el color primario
            AppbarText = "rgb(255, 255, 255)",      // Texto claro para visibilidad
            TextPrimary = "rgb(33, 33, 33)",        // Texto principal oscuro para legibilidad
            TextSecondary = "rgb(117, 117, 117)",   // Texto secundario en tono gris moderado
            DrawerBackground = "rgb(245, 245, 245)",// Fondo suave para menús laterales
            DrawerText = "rgb(33, 33, 33)"          // Texto oscuro para contraste
        },
        PaletteDark = new PaletteDark
        {
            Primary = "rgb(34, 150, 243)",
            Secondary = "rgb(255, 87, 34)",
            Background = "rgb(18, 18, 18)",         // Fondo oscuro para la versión dark
            Surface = "rgb(34, 34, 34)",             // Superficie oscura pero con detalles
            AppbarBackground = "rgb(34, 150, 243)",
            AppbarText = "rgb(255, 255, 255)",
            TextPrimary = "rgb(255, 255, 255)",
            TextSecondary = "rgb(189, 189, 189)",
            DrawerBackground = "rgb(27, 27, 27)",
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