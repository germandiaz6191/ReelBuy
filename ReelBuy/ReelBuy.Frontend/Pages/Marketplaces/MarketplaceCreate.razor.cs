using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.Resources;
using ReelBuy.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace ReelBuy.Frontend.Pages.Marketplaces;

public partial class MarketplaceCreate
{
    private MarketplaceForm? marketplaceForm;
    private Marketplace marketplace = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    private async Task CreateAsync()
    {
        var responseHttp = await Repository.PostAsync("/api/marketplaces", marketplace);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message], Severity.Error);
            return;
        }

        Return();
        Snackbar.Add(Localizer["RecordCreatedOk"], Severity.Success);
    }

    private void Return()
    {
        marketplaceForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("/marketplaces");
    }
}