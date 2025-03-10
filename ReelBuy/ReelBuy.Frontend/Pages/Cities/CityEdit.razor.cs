using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.Cities;

public partial class CityEdit
{
    private City? city;
    private CityForm? cityForm;
    private List<Department>? departments;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadDepartmentsAsync();
        await LoadCitiesAsync();
    }

    private async Task EditAsync()
    {
        var cityDTO = new CityDTO()
        {
            DepartmentId = city!.Department!.Id,
            Id = city.Id,
            Name = city.Name
        };
        var responseHttp = await Repository.PutAsync("api/cities/full", cityDTO);

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
        cityForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("cities");
    }

    private async Task LoadDepartmentsAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Department>>("/api/departments/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        departments = responseHttp.Response;
    }

    private async Task LoadCitiesAsync()
    {
        var responseHttp = await Repository.GetAsync<City>($"api/cities/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("cities");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            city = responseHttp.Response;
        }
    }
}