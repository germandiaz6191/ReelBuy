using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.Departments;

public partial class DepartmentEdit
{
    private Department? department;
    private DepartmentForm? departmentForm;
    private List<Country>? countries;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadCountriesAsync();
        await LoadDepartmentAsync();
    }

    private async Task EditAsync()
    {
        var departmentDTO = new DepartmentDTO()
        {
            CountryId = department!.Country!.Id,
            Department = department!
        };

        var responseHttp = await Repository.PutAsync("api/departments/full", departmentDTO);

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        Return();
        Snackbar.Add(Localizer["RecordSavedOk"], Severity.Success);
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

    private async Task LoadDepartmentAsync()
    {
        var responseHttp = await Repository.GetAsync<Department>($"api/departments/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("departments");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            department = responseHttp.Response;
        }
    }

    private void Return()
    {
        departmentForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("departments");
    }
}