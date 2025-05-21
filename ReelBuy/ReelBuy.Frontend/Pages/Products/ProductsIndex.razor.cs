using System.Net;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.Resources;
using ReelBuy.Frontend.Shared;
using ReelBuy.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Shared.DTOs;

namespace ReelBuy.Frontend.Pages.Products;

public partial class ProductsIndex
{
    private User? user;
    private List<Product>? Products { get; set; }
    private MudTable<Product> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/products";
    private const string baseUrlFavorite = "api/favorites";
    private string infoFormat = "{first_item}-{last_item} => {all_items}";

    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserAsyc();
        await LoadTotalRecordsAsync();
    }

    private async Task LoadTotalRecordsAsync()
    {
        loading = true;
        
        if (user == null)
        {
            await LoadUserAsyc();
        }
        
        if (user == null)
        {
            loading = false;
            return;
        }
        
        // Comprobar si el usuario es administrador
        bool isAdmin = user.ProfileId == 1; // Asumiendo que el ProfileId 1 es para administradores
        
        string url = $"{baseUrl}/totalRecordsPaginated";
        
        // Para usuarios normales, filtrar por tiendas
        if (!isAdmin)
        {
            var storesResponse = await Repository.GetAsync<List<Store>>($"/api/stores/user/{user.Id}");
            if (storesResponse.Error || storesResponse.Response == null || !storesResponse.Response.Any())
            {
                loading = false;
                totalRecords = 0;
                return;
            }
            
            var storeIds = string.Join(",", storesResponse.Response.Select(s => s.Id));
            url += $"?storeIds={storeIds}";
            
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }
        }
        else
        {
            // Para administradores, no filtrar por tiendas
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"?filter={Filter}";
            }
        }

        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            loading = false;
            return;
        }

        totalRecords = responseHttp.Response;
        loading = false;
    }

    private Func<TableState, CancellationToken, Task<TableData<Product>>> LoadListAsync => async (state, cancellationToken) =>
    {
        if (user == null)
        {
            await LoadUserAsyc();
        }

        if (user == null)
        {
            return new TableData<Product> { Items = [], TotalItems = 0 };
        }
        
        // Comprobar si el usuario es administrador
        bool isAdmin = user.ProfileId == 1; // Asumiendo que el ProfileId 1 es para administradores
        
        // Para administradores, cargar todos los productos sin filtrar por tienda
        if (isAdmin)
        {
            int adminPage = state.Page + 1;
            int adminPageSize = state.PageSize;
            string adminUrl = $"{baseUrl}/paginated/?page={adminPage}&recordsnumber={adminPageSize}";

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                adminUrl += $"&filter={Filter}";
            }
            
            var adminResponse = await Repository.GetAsync<List<Product>>(adminUrl);
            if (adminResponse.Error)
            {
                var message = await adminResponse.GetErrorMessageAsync();
                Snackbar.Add(Localizer[message!], Severity.Error);
                return new TableData<Product> { Items = [], TotalItems = 0 };
            }
            
            if (adminResponse.Response == null)
            {
                return new TableData<Product> { Items = [], TotalItems = 0 };
            }
            
            // Cargar las relaciones necesarias
            await LoadStoresForProducts(adminResponse.Response);
            await LoadCategoryAndMarketplaceForProducts(adminResponse.Response);
            
            return new TableData<Product>
            {
                Items = adminResponse.Response,
                TotalItems = totalRecords
            };
        }
        
        // Para usuarios normales, obtener solo las tiendas del usuario
        var storesResponse = await Repository.GetAsync<List<Store>>($"/api/stores/user/{user.Id}");
        if (storesResponse.Error)
        {
            var message = await storesResponse.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return new TableData<Product> { Items = [], TotalItems = 0 };
        }

        var userStores = storesResponse.Response;
        if (userStores == null || !userStores.Any())
        {
            Snackbar.Add(Localizer["NoStoresFound"], Severity.Info);
            return new TableData<Product> { Items = [], TotalItems = 0 };
        }

        // Obtener los productos de las tiendas del usuario
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        var storeIds = string.Join(",", userStores.Select(s => s.Id));
        var url = $"{baseUrl}/paginated/?page={page}&recordsnumber={pageSize}&storeIds={storeIds}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Product>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return new TableData<Product> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<Product> { Items = [], TotalItems = 0 };
        }

        // Incluir las relaciones necesarias
        var products = responseHttp.Response;
        foreach (var product in products)
        {
            product.Store = userStores.FirstOrDefault(s => s.Id == product.StoreId);
            if (product.Store != null)
            {
                product.Store.Products = products.Where(p => p.StoreId == product.StoreId).ToList();
            }
        }

        // Cargar información de categoría y marketplace
        await LoadCategoryAndMarketplaceForProducts(products);

        return new TableData<Product>
        {
            Items = products,
            TotalItems = totalRecords
        };
    };
    
    // Método para cargar las tiendas para productos
    private async Task LoadStoresForProducts(List<Product> products)
    {
        // Obtener todas las tiendas necesarias
        var storeIds = products.Select(p => p.StoreId).Distinct().ToList();
        if (!storeIds.Any()) return;
        
        // Cargar información de tiendas una por una
        foreach (var storeId in storeIds)
        {
            var storeResponse = await Repository.GetAsync<Store>($"/api/stores/{storeId}");
            if (!storeResponse.Error && storeResponse.Response != null)
            {
                var store = storeResponse.Response;
                foreach (var product in products.Where(p => p.StoreId == storeId))
                {
                    product.Store = store;
                }
            }
        }
    }

    // Método para cargar categorías y marketplaces para productos
    private async Task LoadCategoryAndMarketplaceForProducts(List<Product> products)
    {
        // Obtener todas las categorías y marketplaces necesarios
        var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
        var marketplaceIds = products.Select(p => p.MarketplaceId).Distinct().ToList();
        
        // Cargar información de categorías
        foreach (var categoryId in categoryIds)
        {
            var categoryResponse = await Repository.GetAsync<Category>($"/api/categories/{categoryId}");
            if (!categoryResponse.Error && categoryResponse.Response != null)
            {
                var category = categoryResponse.Response;
                foreach (var product in products.Where(p => p.CategoryId == categoryId))
                {
                    product.Category = category;
                }
            }
        }
        
        // Cargar información de marketplaces
        foreach (var marketplaceId in marketplaceIds)
        {
            var marketplaceResponse = await Repository.GetAsync<Marketplace>($"/api/marketplaces/{marketplaceId}");
            if (!marketplaceResponse.Error && marketplaceResponse.Response != null)
            {
                var marketplace = marketplaceResponse.Response;
                foreach (var product in products.Where(p => p.MarketplaceId == marketplaceId))
                {
                    product.Marketplace = marketplace;
                }
            }
        }
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    private async Task ShowModalAsync(int id = 0, bool isEdit = false)
    {
        var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
        IDialogReference? dialog;
        if (isEdit)
        {
            var parameters = new DialogParameters
                {
                    { "Id", id }
                };
            dialog = DialogService.Show<ProductEdit>($"{Localizer["Edit"]} {Localizer["Product"]}", parameters, options);
        }
        else
        {
            dialog = DialogService.Show<ProductCreate>($"{Localizer["New"]} {Localizer["Product"]}", options);
        }

        var result = await dialog.Result;
        if (result!.Canceled)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    private async Task DeleteAsync(Product product)
    {
        var parameters = new DialogParameters
            {
                { "Message", string.Format(Localizer["DeleteConfirm"], product.Name) }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = DialogService.Show<ConfirmDialog>(Localizer["Confirmation"], parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"{baseUrl}/{product.Id}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/products");
            }
            else
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(Localizer[message!], Severity.Error);
            }
            return;
        }
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
        Snackbar.Add(Localizer["RecordDeletedOk"], Severity.Success);
    }

    private async Task DeleteFavoriteAsync(ICollection<Favorite> favorites)
    {
        var foundFavorite = favorites.FirstOrDefault(f => f.UserId == user?.Id);
        var responseHttp = await Repository.DeleteAsync($"{baseUrlFavorite}/{foundFavorite?.Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/products");
            }
            else
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(Localizer[message!], Severity.Error);
            }
            return;
        }
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
        Snackbar.Add(Localizer["RecordDeletedOk"], Severity.Success);
    }

    private async Task AddFavoriteAsync(Product product)
    {
        if (user == null)
        {
            var message = Localizer["FavoriteErrorUserNull"];
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        FavoriteDTO addFavorite = new FavoriteDTO { ProductId = product.Id, UserId = user.Id };
        var responseHttp = await Repository.PostAsync("/api/favorites/full", addFavorite);

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/products");
            }
            else
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(Localizer[message!], Severity.Error);
            }
            return;
        }
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
        Snackbar.Add(Localizer["RecordDeletedOk"], Severity.Success);
    }

    private async Task LoadUserAsyc()
    {
        var responseHttp = await Repository.GetAsync<User>($"/api/accounts");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/");
                return;
            }
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }
        user = responseHttp.Response;
        loading = false;
    }
}