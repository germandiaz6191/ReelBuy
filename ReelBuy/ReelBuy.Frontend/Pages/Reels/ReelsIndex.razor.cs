using System.Net;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.Resources;
using ReelBuy.Frontend.Shared;
using ReelBuy.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Microsoft.AspNetCore.Authorization;

namespace ReelBuy.Frontend.Pages.Reels;


public partial class ReelsIndex
{
    private List<Reel>? Reels { get; set; }
    private MudTable<Reel> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/reels";
    private string infoFormat = "{first_item}-{last_item} => {all_items}";

    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadTotalRecordsAsync();
    }

    private async Task LoadTotalRecordsAsync()
    {
        loading = true;
        var url = $"{baseUrl}/totalRecordsPaginated";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"?filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        totalRecords = responseHttp.Response;
        loading = false;
    }

    private Func<TableState, CancellationToken, Task<TableData<Reel>>> LoadListAsync => async (state, cancellationToken) =>
    {
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        var url = $"{baseUrl}/paginated/?page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Reel>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return new TableData<Reel> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null)
        {
            return new TableData<Reel> { Items = [], TotalItems = 0 };
        }
        return new TableData<Reel>
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
            dialog = DialogService.Show<ReelEdit>($"{Localizer["Edit"]} {Localizer["Reel"]}", parameters, options);
        }
        else
        {
            dialog = DialogService.Show<ReelCreate>($"{Localizer["New"]} {Localizer["Reel"]}", options);
        }

        var result = await dialog.Result;
        if (result!.Canceled)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    [Authorize(Roles = "Admin,Seller")]
    private async Task DeleteAsync(Reel reel)
    {
        var parameters = new DialogParameters
            {
                { "Message", string.Format(Localizer["DeleteConfirm"], reel.Name) }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = DialogService.Show<ConfirmDialog>(Localizer["Confirmation"], parameters, options);
        var result = await dialog.Result;
        if (result!.Canceled)
        {
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"{baseUrl}/{reel.Id}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/reels");
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

    private async Task ShowReelModalAsync(string? videoUrl)
    {
        if (string.IsNullOrWhiteSpace(videoUrl))
            return;

        var parameters = new DialogParameters
        {
            ["VideoUrl"] = videoUrl,
            ["Label"] = Localizer["Reel"]
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };

        await DialogService.ShowAsync(typeof(ReelBuy.Frontend.Shared.PreviewReel), "Previsualización", parameters, options);
    }
}