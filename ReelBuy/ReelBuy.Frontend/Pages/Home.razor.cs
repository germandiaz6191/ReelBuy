using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;
using System.Collections.Generic;

namespace ReelBuy.Frontend.Pages;

public partial class Home
{
    private List<Reel> dummyReels = new();
    private List<Product> allFetchedReels = new();
    private List<Product> currentDisplayVideos = new();
    private int totalRecords = 0;

    private const string baseUrl = "api/products";

    private Reel? selectedReel = null;

    private int currentBatch = 0;
    private bool loading = false;
    private bool isLastBatch = false;

    private bool showFooter = true;

    // Tamaños configurables
    private const int BatchSize = 10; // Videos por consulta

    private const int DisplaySize = 4; // Videos a mostrar

    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        loading = true;
        await LoadProductsAsync();
        await LoadTotalRecordsAsync();
        loading = false;
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

    private async Task LoadProductsAsync()
    {
        // Simula carga desde API (reemplaza con tu lógica real)
        List<Product> newReels = await FetchVideosFromApi(currentBatch, BatchSize);

        /* Reel reel = new Reel
         {
             Title = "Video 1",
             Thumbnail = "https://via.placeholder.com/150",
             Link = "#"
         };

         Reel reel2 = new Reel
         {
             Title = "Video 2",
             Thumbnail = "https://via.placeholder.com/150",
             Link = "#"
         };

         Reel reel3 = new Reel
         {
             Title = "Video 3",
             Thumbnail = "https://via.placeholder.com/150",
             Link = "#"
         };

         Reel reel4 = new Reel
         {
             Title = "Video 4",
             Thumbnail = "https://www.youtube.com/watch?v=lgmW-HIrth8",
             Link = "#"
         };

         Reel reel5 = new Reel
         {
             Title = "Video 5",
             Thumbnail = "https://www.youtube.com/watch?v=lgmW-HIrth8",
             Link = "#"
         };

         dummyReels.Add(reel);
         dummyReels.Add(reel2);
         dummyReels.Add(reel3);
         dummyReels.Add(reel4);
         dummyReels.Add(reel5);

         var newReels = dummyReels;
        */

        if (newReels.Count == 0)
        {
            return;
        }

        allFetchedReels.AddRange(newReels);
        UpdateDisplayVideos();
        isLastBatch = newReels.Count < BatchSize;
    }

    private async Task<List<Product>> FetchVideosFromApi(int currentBatch, int batchSize)
    {
        int page = currentBatch + 1;
        int pageSize = batchSize;
        var url = $"{baseUrl}/paginated/?page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await Repository.GetAsync<List<Product>>(url);
        if (responseHttp.Response == null || responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return new List<Product>();
        }

        return responseHttp.Response;
    }

    private async Task FetchVideoBatch()
    {
        await Task.Delay(5);
        var newReels = await FetchVideosFromApi(currentBatch * BatchSize, BatchSize);

        allFetchedReels.AddRange(newReels);
        UpdateDisplayVideos();

        isLastBatch = newReels.Count < BatchSize;
    }

    private void UpdateDisplayVideos()
    {
        currentDisplayVideos = allFetchedReels
            .Skip(currentBatch * DisplaySize)
            .Take(DisplaySize)
            .ToList();
    }

    public async Task LoadNextBatch()
    {
        if ((currentBatch + 1) * DisplaySize >= allFetchedReels.Count)
        {
            await FetchVideoBatch(); // Carga nuevos 10 videos si es necesario
        }
        currentBatch++;
        UpdateDisplayVideos();
    }

    public void LoadPreviousBatch()
    {
        currentBatch = Math.Max(0, currentBatch - 1);
        UpdateDisplayVideos();
    }

    private void SelectReel(Reel reel)
    {
        selectedReel = reel;
        StateHasChanged(); // Forzar actualización de la UI
    }

    private void ToggleFooter()
    {
        showFooter = !showFooter;
    }
}