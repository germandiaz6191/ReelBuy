using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.Cities;

public partial class CityForm
{
    private EditContext editContext = null!;

    private Department selectedDepartment = new();

    protected override void OnInitialized()
    {
        editContext = new(City);

        if (City.DepartmentId != null && City.DepartmentId != 0)
        {
            var departmentId = City!.DepartmentId;
            var department = filterDepartment(departmentId);
            selectedDepartment = department;
            City.Department = department;
            City.DepartmentId = departmentId;
        }
    }

    [EditorRequired, Parameter] public City City { get; set; } = null!;
    [EditorRequired, Parameter] public List<Department> Departments { get; set; } = null!;
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

    private Func<string, Task<IEnumerable<Department>>> SearchDepartment => async (searchText) =>
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Departments!;
        }

        return Departments!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    };

    private void DepartmentChanged(Department department)
    {
        selectedDepartment = department;
        City.Department = department;
        City.DepartmentId = department.Id;

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

    private Department filterDepartment(int? id)
    {
        return Departments.FirstOrDefault(x => x.Id == id) ?? new();
    }
}