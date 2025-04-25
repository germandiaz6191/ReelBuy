using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Frontend.Shared;
using ReelBuy.Shared.Enums;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.Products.Pending;

public partial class ProductsPending
{
    private bool loading = false;
    private int TotalRecords = 0;

    private const string baseUrl = "api/products";
    [Parameter, SupplyParameterFromQuery] public int? FilterStatus { get; set; } = null;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        loading = true;
        await LoadRecordsProductsPendingAsync();
        loading = false;
    }

    private async Task LoadRecordsProductsPendingAsync()
    {
        var url = $"{baseUrl}/totalRecordsPaginated";
        FilterStatus = (int)StatusProduct.Pending;
        url += $"?filterStatus={FilterStatus}";

        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        TotalRecords = responseHttp.Response;
    }
}