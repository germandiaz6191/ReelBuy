using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.Departments;

public partial class DepartmentForm
{
    private EditContext editContext = null!;

    private Country selectedCountry = new();

    protected override void OnInitialized()
    {
        editContext = new(Department);

        if (Department.CountryId != null && Department.CountryId != 0)
        {
            var countryId = Department.CountryId;
            var country = filterCountry(countryId);
            selectedCountry = country;
            Department.Country = country;
            Department.CountryId = countryId;
        }
    }

    [EditorRequired, Parameter] public Department Department { get; set; } = null!;
    [EditorRequired, Parameter] public List<Country> Countries { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

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

    private Func<string, CancellationToken, Task<IEnumerable<Country>>> SearchCountries => async (searchText, cancellationToken) =>
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Countries!;
        }

        return Countries!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    };

    private void CountryChanged(Country country)
    {
        selectedCountry = country;
        Department.Country = country;
        Department.CountryId = country.Id;

        CheckValidation();
    }

    private async Task HandleValidSubmit()
    {
        await OnValidSubmit.InvokeAsync();
    }

    private void OnFieldBlur()
    {
        CheckValidation();
    }

    private async void CheckValidation()
    {
        await Task.Delay(50);
        editContext.Validate();
    }

    private Country filterCountry(int? id)
    {
        return Countries.FirstOrDefault(x => x.Id == id) ?? new();
    }
}