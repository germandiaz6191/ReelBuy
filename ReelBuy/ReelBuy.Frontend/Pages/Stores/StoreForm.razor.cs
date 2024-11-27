using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.Stores;

public partial class StoreForm
{
    private EditContext editContext = null!;

    private City selectedCity = new();

    protected override void OnInitialized()
    {
        editContext = new(Store);

        if (Store.CityId != null && Store.CityId != 0)
        {
            var cityId = Store!.CityId;
            var city = filterCity(cityId);
            selectedCity = city;
            Store.City = city;
            Store.CityId = cityId;
        }
    }

    [EditorRequired, Parameter] public Store Store { get; set; } = null!;
    [EditorRequired, Parameter] public List<City> Cities { get; set; } = null!;
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
    private async Task<IEnumerable<City>> SearchCity(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Cities!;
        }

        return Cities!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }
    private void CityChanged(City city)
    {
        selectedCity = city;
        Store.City = city;
        Store.CityId = city.Id;

        CheckValidation();
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
    private City filterCity(int? id)
    {
        return Cities.FirstOrDefault(x => x.Id == id) ?? new();
    }
}