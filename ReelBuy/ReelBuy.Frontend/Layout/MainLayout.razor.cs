using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;
using static MudBlazor.Colors;

namespace ReelBuy.Frontend.Layout;

public partial class MainLayout
{
    private bool _drawerOpen = false;
    private string _icon = Icons.Material.Filled.DarkMode;
    private string? selectedProduct;
    private List<string> _searchResults = [];
    private const string baseUrl = "api/products/search";

    private bool DarkMode { get; set; } = true;

    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private MudTheme CustomTheme => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "rgb(74, 81, 94)",
            Secondary = "rgb(63, 81, 181)",
            Background = "rgb(245, 245, 245)",
            Surface = "rgb(255, 255, 255)",
            AppbarBackground = "rgb(74, 81, 94)",
            AppbarText = "rgb(255, 255, 255)",
            TextPrimary = "rgb(169, 169, 169)",
            TextSecondary = "rgb(33, 33, 33)",
            DrawerBackground = "rgb(238, 238, 238)",
            DrawerText = "rgb(33, 33, 33)",
            SecondaryDarken = "#f8f9fa" //comments
        },
        PaletteDark = new PaletteDark
        {
            Primary = "rgb(74, 81, 94)",
            Secondary = "rgb(255, 159, 64)",
            //Background = "rgb(18, 18, 18)",
            Background = "rgb(219, 218, 216)", //page
            //Surface = "rgb(51, 51, 51)",
            Surface = "#fff",
            //AppbarBackground = "rgb(74, 81, 94)",
            AppbarBackground = "#460C61", //header
            AppbarText = "rgb(255, 255, 255)",
            TextPrimary = "#460C61", //input text
            TextSecondary = "#460C61", //titulos text
            LinesInputs = "#460C61", //input line down
            LinesDefault = "#fff", //input line whit back blue
            //DrawerBackground = "rgb(30, 30, 30)",
            DrawerBackground = "#fff", //menu
            //DrawerText = "rgb(255, 255, 255)"
            DrawerText = "#460C61",
            DrawerIcon = "#460C61",
            SecondaryDarken = "#f8f9fa" //comments
        }
    };

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private void DarkModeToggle()
    {
        DarkMode = !DarkMode;
        _icon = DarkMode ? Icons.Material.Filled.LightMode : Icons.Material.Filled.DarkMode;
    }

    private async Task<IEnumerable<string>> SearchProduct(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText) || searchText.Length <= 3)
        {
            return [];
        }

        return await PerformSearch(searchText);
    }

    private async Task<List<string>> PerformSearch(string query)
    {
        var allResults = await GetSearchResults(query);

        _searchResults = allResults;

        if (allResults.Count == 0)
        {
            _searchResults.Add(Localizer["PrincipalSearchNotFoundResults"]);
        }
        if (allResults.Count > 10)
        {
            _searchResults = allResults.Take(10).ToList();
            _searchResults.Add(Localizer["PrincipalSearchExceededMaxResults"]);
        }

        return _searchResults;
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
            return [];
        }

        return responseHttp.Response?.Select(product => product.Name).ToList() ?? [];
    }

    private void ProductChanged(string product)
    {
        if (product.Contains(Localizer["PrincipalSearchNotFoundResults"])) { return; }
        
        selectedProduct = product;
        NavigationManager.NavigateTo($"/?search={Uri.EscapeDataString(product)}");
    }

    private string BuildThemeCssVars()
    {
        Palette p = DarkMode ? CustomTheme.PaletteDark : CustomTheme.PaletteLight;

        return $@"
        --color-primary: {p.Primary};
        --color-secondary: {p.Secondary};
        --color-background: {p.Background};
        --color-surface: {p.Surface};
        --color-text-primary: {p.TextPrimary};
        --color-text-secondary: {p.TextSecondary};
        --color-appbar-background: {p.AppbarBackground};
        --color-appbar-text: {p.AppbarText};
        --color-drawer-background: {p.DrawerBackground};
        --color-drawer-text: {p.DrawerText};
        --color-lines-inputs: {p.LinesInputs};
        --color-lines-default: {p.LinesDefault};
        --color-secondary-darken: {p.SecondaryDarken};

    ";
    }
}