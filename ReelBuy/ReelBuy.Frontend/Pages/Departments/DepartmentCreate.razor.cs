using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.Departments;

public partial class DepartmentCreate
{
    private DepartmentForm? departmentForm;
    private Department department = new();
    private List<Country>? countries;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await LoadCountriesAsync();
    }

    private async Task LoadCountriesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Country>>("/api/countries/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        countries = responseHttp.Response;
    }

    private async Task CreateAsync()
    {
        Console.WriteLine("LLego el evento");
        var departmentDTO = new DepartmentDTO()
        {
            CountryId = department!.Country!.Id,
            Department = department!
        };
        Console.WriteLine("consumo de servicio");
        var responseHttp = await Repository.PostAsync("/api/departments/full", departmentDTO);
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
        departmentForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("/departments");
    }
}