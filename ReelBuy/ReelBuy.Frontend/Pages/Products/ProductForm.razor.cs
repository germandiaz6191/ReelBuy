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
    private Status selectedStatus = new();
    private List<Status>? statuses;
    private Category selectedCategory = new();
    private List<Category>? categories;
    private Marketplace selectedMarketplace = new();
    private List<Marketplace>? marketplaces;
    private Product product = new();

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
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadStatusesAsync();
        await LoadCategoriesAsync();
        await LoadMarketplacesAsync();

        selectedStatus = product!.Status!;
        selectedCategory = product!.Category!;
        selectedMarketplace = product!.Marketplace!;
    }

    private void StatusChanged(Status status)
    {
        selectedStatus = status;
    }

    private void CategoryChanged(Category category)
    {
        selectedCategory = category;
    }

    private void MarketplaceChanged(Marketplace marketplace)
    {
        selectedMarketplace = marketplace;
    }

    private async Task<IEnumerable<Status>> SearchStatuses(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return statuses!;
        }

        return statuses!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Category>> SearchCategories(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return categories!;
        }

        return categories!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task<IEnumerable<Marketplace>> SearchMarketplaces(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return marketplaces!;
        }

        return marketplaces!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private async Task LoadStatusesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Status>>("/api/statuses/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        statuses = responseHttp.Response;
    }

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

    private async Task SaveProductAsync()
    {
        product!.Status = selectedStatus;
        product!.StatusId = selectedStatus.Id;
        product!.Category = selectedCategory;
        product!.CategoryId = selectedCategory.Id;
        product!.Marketplace = selectedMarketplace;
        product!.MarketplaceId = selectedMarketplace.Id;
        product!.Name = Product.Name;

        var responseHttp = await Repository.PostAsync("/api/products", product!);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        Snackbar.Add(Localizer["RecordSavedOk"], Severity.Success);
        NavigationManager.NavigateTo("/");
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
}