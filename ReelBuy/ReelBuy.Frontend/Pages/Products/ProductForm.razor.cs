using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ReelBuy.Shared.Resources;
using ReelBuy.Shared.Entities;
using ReelBuy.Frontend.Repositories;
using MudBlazor;

namespace ReelBuy.Frontend.Pages.Products;

public partial class ProductForm
{
    private EditContext editContext = null!;
    private Category selectedCategory = new();
    private List<Category>? categories;
    private Marketplace selectedMarketplace = new();
    private List<Marketplace>? marketplaces;
    private Product product = new();
    private Store selectedStore = new();
    private List<Store>? stores;
    private string? reelBase64;
    private string? reelUrl;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [EditorRequired, Parameter] public Product Product { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    protected override void OnInitialized()
    {
        editContext = new(Product);
        product = Product;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadCategoriesAsync();
        await LoadMarketplacesAsync();
        await LoadStoresAsync();

        selectedCategory = product!.Category!;
        selectedMarketplace = product!.Marketplace!;
        selectedStore = product!.Store!;
        reelUrl = product.Reels.FirstOrDefault()?.ReelUri;
        reelBase64 = string.IsNullOrEmpty(product.Reels?.FirstOrDefault()?.ReelUri) ? string.Empty : product.Reels.FirstOrDefault()?.ReelUri;
    }

    private void CategoryChanged(Category category)
    {
        selectedCategory = category;
    }

    private void MarketplaceChanged(Marketplace marketplace)
    {
        selectedMarketplace = marketplace;
    }

    private void StoreChanged(Store store)
    {
        selectedStore = store;
    }

    private Func<string, CancellationToken, Task<IEnumerable<Category>>> SearchCategories => async (searchText, cancellationToken) =>
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return categories!;
        }

        return categories!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
        .ToList();
    };

    private Func<string, CancellationToken, Task<IEnumerable<Marketplace>>> SearchMarketplaces => async (searchText, cancellationToken) =>
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return marketplaces!;
        }

        return marketplaces!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    };

    private Func<string, CancellationToken, Task<IEnumerable<Store>>> SearchStores => async (searchText, cancellationToken) =>
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return stores!;
        }

        return stores!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    };

    private async Task LoadCategoriesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Category>>("/api/categories/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        categories = responseHttp.Response;
    }

    private async Task LoadMarketplacesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Marketplace>>("/api/marketplaces/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        marketplaces = responseHttp.Response;
    }

    private async Task LoadStoresAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Store>>("/api/stores/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        stores = responseHttp.Response;
    }

    private async Task SaveProductAsync()
    {
        product!.CategoryId = selectedCategory.Id;
        product!.MarketplaceId = selectedMarketplace.Id;
        product!.Name = Product.Name;
        product!.Description = Product.Description;
        product!.Price = Product.Price;
        product!.StoreId = selectedStore.Id;

        // Limpiar las entidades relacionadas para evitar errores de validación
        product!.Category = null;
        product!.Marketplace = null;
        product!.Store = null;
        product!.Status = null;
        product!.Favorites = null;
        product!.Comments = null;
        product!.LikedBy = null;

        if(product.Id == 0)
        {
            product!.Reels.Add(CreateReel());
        }
       
        // Determinar si es creación o edición basado en el Id
        var responseHttp = product.Id == 0
            ? await Repository.PostAsync("/api/products", product!)
            : await Repository.PutAsync("/api/products", product!);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Return();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        Return();
        Snackbar.Add(Localizer["RecordSavedOk"], Severity.Success);
    }

    private void Return()
    {
        FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("/products");
    }

    public bool FormPostedSuccessfully { get; set; } = false;

    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formWasEdited = editContext.IsModified();

        if (!formWasEdited || FormPostedSuccessfully)
        {
            return;
        }

        var result = await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"],
            Text = Localizer["LeaveAndLoseChanges"],
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true
        });

        var confirm = !string.IsNullOrEmpty(result.Value);
        if (confirm)
        {
            return;
        }

        context.PreventNavigation();
    }

    private void ReelSelected(string _reelUri)
    {
        reelUrl = _reelUri;
    }

    private Reel CreateReel()
    {
        var reel = new Reel
        {
            Name = product.Name + "-reel-product",
            ProductId = product!.Id,
            ReelUri = reelUrl!
        };
        return reel;
    }
}