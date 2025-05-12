using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.Resources;
using ReelBuy.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Shared.Enums;

namespace ReelBuy.Frontend.Pages.Products.Pending;

public partial class ProductsPendingIndex
{
    private List<Product>? Products { get; set; }
    private List<Status>? Statuses = new();
    private List<Product> modifiedProducts = new();
    private MudTable<Product> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/products";
    private const string baseUrlStatuses = "api/statuses";
    private string infoFormat = "{first_item}-{last_item} => {all_items}";
    private bool isModalOpen = false;
    private Product? selectedProduct = null;

    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;
    [Parameter, SupplyParameterFromQuery] public int? FilterStatus { get; set; } = null;
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var loadStatusesTask = LoadStatusesAsync();
        var loadTotalRecordsTask = LoadTotalRecordsAsync();

        await Task.WhenAll(loadStatusesTask, loadTotalRecordsTask);

        if (table is not null)
        {
            await table.ReloadServerData();
        }
    }

    private async Task LoadStatusesAsync()
    {
        var url = $"{baseUrlStatuses}/combo";
        var responseHttp = await Repository.GetAsync<List<Status>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        Statuses = responseHttp.Response;
    }

    private async Task LoadTotalRecordsAsync()
    {
        loading = true;
        var url = $"{baseUrl}/totalRecordsPaginated";

        FilterStatus = (int)StatusProduct.Pending;
        url += $"?filterStatus={FilterStatus}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        totalRecords = responseHttp.Response;
        loading = false;
    }

    private Func<TableState, CancellationToken, Task<TableData<Product>>> LoadListAsync => async (state, cancellationToken) =>
    {
        // Obtener los productos de las tiendas del usuario
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        var url = $"{baseUrl}/paginated/?page={page}&recordsnumber={pageSize}";

        FilterStatus = (int)StatusProduct.Pending;
        url += $"&filterStatus={FilterStatus}";

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

        return new TableData<Product>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    };

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    private async Task UpdateProductsAsync()
    {
        loading = true;

        var url = $"{baseUrl}/UpdateProducts";
        var responseHttp = await Repository.PutAsync(url, modifiedProducts);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        loading = false;
        Snackbar.Add(Localizer["RecordUpdateOk"], Severity.Success);
    }

    private void ShowPreviewDialog(Product product)
    {
        selectedProduct = product;
        isModalOpen = true;
    }

    private void CloseModal()
    {
        selectedProduct = null;
        isModalOpen = false;
    }

    private void ShowInfo()
    {
    }

    private void OnStatusChanged(Product product, int newStatusId)
    {
        product.StatusId = newStatusId;
        product.Favorites = null;
        product.Store = null;
        if (product.Status != null)
        {
            product.Status.Products = new List<Product> { null };
        }

        if (product.Category != null)
        {
            product.Category.Products = new List<Product> { null };
        }

        if (product.Marketplace != null)
        {
            product.Marketplace.Products = new List<Product> { null };
        }

        if (!modifiedProducts.Any(p => p.Id == product.Id))
        {
            modifiedProducts.Add(product);
        }
    }
}