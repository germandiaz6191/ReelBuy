using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.Stores;

public partial class StoreCreate
{
    private StoreForm? storeForm;
    private Store store = new();
    private List<City>? cities;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await LoadCitiesAsync();
    }

    private async Task LoadCitiesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<City>>("/api/cities/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        cities = responseHttp.Response;
    }

    private async Task CreateAsync()
    {
        var storeDTO = new StoreDTO()
        {
            CityId = store!.City!.Id,
            Name = store.Name!
        };
        var responseHttp = await Repository.PostAsync("/api/stores/full", storeDTO);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        Return();
        Snackbar.Add(Localizer["RecordCreatedOk"], Severity.Success);
    }

    private void Return()
    {
        storeForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("/stores");
    }
}