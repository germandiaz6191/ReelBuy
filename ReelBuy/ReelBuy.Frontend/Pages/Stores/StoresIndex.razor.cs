using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Frontend.Shared;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;
using ReelBuy.Shared.Enums;
using System.Net;

namespace ReelBuy.Frontend.Pages.Stores;

[Authorize(Roles = "Admin,Seller")]
public partial class StoresIndex
{
    private List<Store>? Stores { get; set; }
    private MudTable<Store> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private User? user;
    private const string baseUrl = "api/stores";
    private string infoFormat = "{first_item}-{last_item} => {all_items}";

    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserAsync();
        await LoadTotalRecordsAsync();
    }
    
    private async Task LoadUserAsync()
    {
        var responseHttp = await Repository.GetAsync<User>($"/api/accounts");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/");
                return;
            }
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }
        user = responseHttp.Response;
    }

    private async Task LoadTotalRecordsAsync()
    {
        loading = true;

        if (user == null)
        {
            await LoadUserAsync();
            if (user == null)
            {
                loading = false;
                return;
            }
        }

        // Comprobar si el usuario es administrador
        bool isAdmin = user.ProfileId == (int)UserType.Admin; // Asumiendo que el ProfileId 1 es para administradores
        
        string url = $"{baseUrl}/totalRecordsPaginated";

        // Si no es admin, necesitamos filtrar por el usuario
        if (!isAdmin)
        {
            url += $"?userId={user.Id}";
            
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }
        }
        else
        {
            // Para administradores, usar filtro normal si existe
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"?filter={Filter}";
            }
        }

        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            loading = false;
            return;
        }

        totalRecords = responseHttp.Response;
        loading = false;
    }

    private Func<TableState, CancellationToken, Task<TableData<Store>>> LoadListAsync => async (state, cancellationToken) =>
    {
        if (user == null)
        {
            await LoadUserAsync();
            if (user == null)
            {
                return new TableData<Store> { Items = [], TotalItems = 0 };
            }
        }

        // Comprobar si el usuario es administrador
        bool isAdmin = user.ProfileId == (int)UserType.Admin; // Asumiendo que el ProfileId 1 es para administradores
        
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        string url = $"{baseUrl}/paginated?page={page}&recordsnumber={pageSize}";

        // Si no es admin, necesitamos filtrar por el usuario
        if (!isAdmin)
        {
            url += $"&userId={user.Id}";
        }

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Store>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return new TableData<Store> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<Store> { Items = [], TotalItems = 0 };
        }
        return new TableData<Store>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    };

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    private async Task ShowModalAsync(int id = 0, bool isEdit = false)
    {
        var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
        IDialogReference? dialog;
        if (isEdit)
        {
            var parameters = new DialogParameters
                {
                    { "Id", id }
                };
            dialog = DialogService.Show<StoreEdit>($"{Localizer["Edit"]} {Localizer["Store"]}", parameters, options);
        }
        else
        {
            dialog = DialogService.Show<StoreCreate>($"{Localizer["New"]} {Localizer["Store"]}", options);
        }

        var result = await dialog.Result;
        if (result!.Canceled)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    private async Task DeleteAsync(Store store)
    {
        // Verificar que el usuario actual sea dueño de la tienda o admin
        if (user != null && user.ProfileId != (int)UserType.Admin && store.UserId != user.Id)
        {
            Snackbar.Add("No tienes permiso para eliminar una tienda que no te pertenece", Severity.Warning);
            return;
        }
        
        var parameters = new DialogParameters
            {
                { "Message", string.Format(Localizer["DeleteConfirm"], store.Name) }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = DialogService.Show<ConfirmDialog>(Localizer["Confirmation"], parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"{baseUrl}/{store.Id}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/stores");
            }
            else
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(Localizer[message!], Severity.Error);
            }
            return;
        }
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
        Snackbar.Add(Localizer["RecordDeletedOk"], Severity.Success);
    }
}