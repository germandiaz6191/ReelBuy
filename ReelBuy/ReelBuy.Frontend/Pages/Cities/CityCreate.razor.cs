using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.Cities;

public partial class CityCreate
{
    private CityForm? cityForm;
    private City city = new();
    private List<Department>? departments;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await LoadDepartmentsAsync();
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

    private async Task CreateAsync()
    {
        var cityDTO = new CityDTO()
        {
            DepartmentId = city!.Department!.Id,
            Name = city.Name!
            
        };
        var responseHttp = await Repository.PostAsync("/api/cities/full", cityDTO);
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
        cityForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("/cities");
    }
}