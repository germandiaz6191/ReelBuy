using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.Resources;
using ReelBuy.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Shared.DTOs;
using Microsoft.JSInterop;

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
    [Inject] private IJSRuntime JS { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadTotalRecordsAsync();
    }

    private async Task LoadTotalRecordsAsync()
    {
        loading = true;

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

        var videos = responseHttp.Response;

        _ = Task.Run(async () =>
        {
            await CheckPendingStatusesAsync(videos);
        });

        return new TableData<GeneratedVideo>
        {
            Items = videos,
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

    private async Task CheckPendingStatusesAsync(List<GeneratedVideo> videos)
    {
        var pendingVideos = videos.Where(v => v.StatusDetail == "pending").ToList();

        foreach (var video in pendingVideos)
        {
            try
            {
                VideoStatusUpdateRequest dtoVideoStatusUpdate = new()
                {
                    VideoId = video.VideoId
                };
                var responseHttp = await Repository.PostAsync<VideoStatusUpdateRequest, GeneratedVideo>($"{baseUrl}/check-status", dtoVideoStatusUpdate);

                if (responseHttp.Error)
                {
                    var messageError = await responseHttp.GetErrorMessageAsync();
                    Snackbar.Add(messageError!, Severity.Error);
                    return;
                }

                GeneratedVideo generatedVideo = responseHttp.Response;

                // Si el estado cambió, actualízalo
                if (generatedVideo.StatusDetail != video.StatusDetail)
                {
                    video.StatusDetail = generatedVideo.StatusDetail;
                    video.VideoUrl = generatedVideo.VideoUrl;
                    // Forzar renderizado en el hilo principal
                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error actualizando el estado ", e);
            }
        }
    }

    private async Task HandleExpiredDownloadAsync(GeneratedVideo video)
    {
        try
        {
            var response = await Repository.GetAsync<string>($"{baseUrl}/{video.VideoId}");

            if (!response.Error && !string.IsNullOrWhiteSpace(response.Response))
            {
                // Opcional: actualiza la propiedad local
                video.VideoUrl = response.Response;

                // Forzar descarga desde JavaScript
                await JS.InvokeVoidAsync("downloadFileFromUrl", video.VideoUrl, $"video_{video.Id}.mp4");
            }
            else
            {
                Snackbar.Add(Localizer["No se pudo obtener el video."], Severity.Error);
            }
        }
        catch
        {
            Snackbar.Add(Localizer["Error al intentar descargar el video."], Severity.Error);
        }
    }
}