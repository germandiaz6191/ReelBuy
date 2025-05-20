using System.Net;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.Resources;
using ReelBuy.Frontend.Shared;
using ReelBuy.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Shared.DTOs;

namespace ReelBuy.Frontend.Pages.GeneratedVideos;

public partial class GeneratedVideosIndex
{
    private User? user;
    private List<GeneratedVideo>? GeneratedVideos { get; set; }
    private MudTable<GeneratedVideo> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/VideoGeneration";
    private string infoFormat = "{first_item}-{last_item} => {all_items}";

    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserAsyc();
        if (user != null)
        {
            await LoadTotalRecordsAsync();
        }
    }

    private async Task LoadTotalRecordsAsync()
    {
        loading = true;

        if (user == null)
        {
            await LoadUserAsyc();
        }

        if (user == null)
        {
            loading = false;
            return;
        }

        string url = $"{baseUrl}/totalRecordsPaginated";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"?filter={Filter}";
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

    private Func<TableState, CancellationToken, Task<TableData<GeneratedVideo>>> LoadListAsync => async (state, cancellationToken) =>
    {
        if (user == null)
        {
            await LoadUserAsyc();
        }

        if (user == null)
        {
            return new TableData<GeneratedVideo> { Items = [], TotalItems = 0 };
        }

        int page = state.Page + 1;
        int pageSize = state.PageSize;
        string url = $"{baseUrl}/paginated/?page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<GeneratedVideo>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return new TableData<GeneratedVideo> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<GeneratedVideo> { Items = [], TotalItems = 0 };
        }

        return new TableData<GeneratedVideo>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    };

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await table.ReloadServerData();
    }

    private async Task ShowModalAsync(int id = 0, bool isEdit = false)
    {
        if (user == null)
        {
            await LoadUserAsyc();
            if (user == null)
            {
                Snackbar.Add(Localizer["You must be logged in to perform this action"], Severity.Warning);
                return;
            }
        }

        var parameters = new DialogParameters
        {
            { "Id", id },
            { "IsEdit", isEdit }
        };

        var dialog = await DialogService.ShowAsync<GeneratedVideoForm>("Generated Video", parameters);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await table.ReloadServerData();
        }
    }
    
    private async Task LoadUserAsyc()
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
        loading = false;
    }
}