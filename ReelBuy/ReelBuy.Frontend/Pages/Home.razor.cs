using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Layout;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Frontend.Shared;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages;

public partial class Home
{
    // (Layout as MainLayout)!.PageWrapperClass = "custom-override my-page-style";
    // (Layout as MainLayout)!.PageWrapperStyle = "padding: 1rem 0;";

    private List<Reel> dummyReels = new();
    private List<Reel> allFetchedReels = new();
    private List<Reel> currentDisplayVideos = new();
    private int totalRecords = 0;

    private const string baseUrl = "api/stores";

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

    protected override async Task OnInitializedAsync()
    {
        loading = true;
        await LoadTotalRecordsAsync();
        loading = false;
    }

    private async Task LoadTotalRecordsAsync()
    {
        // Simula carga desde API (reemplaza con tu lógica real)
        //var newReels = await FetchVideosFromApi(currentBatch * BatchSize, BatchSize);
        var url = $"{baseUrl}/totalRecordsPaginated";

        var responseHttp = await Repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        Reel reel = new Reel
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

        allFetchedReels.AddRange(newReels);
        UpdateDisplayVideos();
        Console.WriteLine("-----AA----------- " + newReels.Count);
        Console.WriteLine("------EE---------- " + BatchSize);
        isLastBatch = newReels.Count < BatchSize; // ¿Es el último lote?

        totalRecords = responseHttp.Response;
    }

    private async Task FetchVideoBatch()
    {
        // Simula carga desde API (reemplaza con tu lógica real)
        //var newReels = await FetchVideosFromApi(currentBatch * BatchSize, BatchSize);
        var newReels = dummyReels;

        allFetchedReels.AddRange(newReels);
        UpdateDisplayVideos();
        Console.WriteLine("-----CC----------- " + newReels.Count);
        Console.WriteLine("------DD---------- " + BatchSize);
        isLastBatch = newReels.Count < BatchSize; // ¿Es el último lote?
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