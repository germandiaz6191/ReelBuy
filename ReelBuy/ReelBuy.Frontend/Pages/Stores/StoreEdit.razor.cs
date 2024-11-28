using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.Stores;

public partial class StoreEdit
{
    private Store? store;
    private StoreForm? storeForm;
    private List<City>? cities;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadCitiesAsync();
        await LoadStoresAsync();
    }

    private async Task EditAsync()
    {
        var storeDTO = new StoreDTO()
        {
            CityId = store!.City!.Id,
            Id = store.Id,
            Name = store.Name
        };
        var responseHttp = await Repository.PutAsync("api/stores/full", storeDTO);

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        Return();
        Snackbar.Add(Localizer["RecordSavedOk"], Severity.Success);
    }

    private void Return()
    {
        storeForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("stores");
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

    private async Task LoadStoresAsync()
    {
        var responseHttp = await Repository.GetAsync<Store>($"api/stores/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("stores");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            store = responseHttp.Response;
        }
    }
}