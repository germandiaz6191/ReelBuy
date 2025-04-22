using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages;

public partial class Home
{
    private List<Reel> dummyReels = new();
    private List<Product> allFetchedReels = new();
    private int totalRecords = 0;
    private int totalPages = 0;

    private const string baseUrl = "api/products";

    private Reel? selectedReel = null;

    private int CurrentBatch = 1;
    private int CurrentVideoIndex = 0;
    private bool loading = false;

    // Tamaños configurables
    private const int BatchSize = 2; // Videos por consulta

    // Rotador automatico
    private bool isTransitioning = false;

    private bool isLiked = false;

    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        loading = true;
        await LoadProductsAsync();
        await LoadTotalRecordsAsync();
        CalculatePages();
        loading = false;
    }

    private void CalculatePages()
    {
        totalPages = (int)Math.Ceiling((double)totalRecords / BatchSize);
    }

    private async Task LoadTotalRecordsAsync()
    {
        var url = $"{baseUrl}/totalRecordsPaginatedApproved";

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
    }

    private async Task LoadProductsAsync()
    {
        // Simula carga desde API (reemplaza con tu lógica real)
        List<Product> newReels = await FetchVideosFromApi(CurrentBatch, BatchSize);

        if (newReels.Count == 0)
        {
            return;
        }

        allFetchedReels.AddRange(newReels);
        await SelectVideo(CurrentVideoIndex);
    }

    private async Task SelectVideo(int currentVideoIndex)
    {
        await Task.Delay(5);
        var firstReel = allFetchedReels[currentVideoIndex]?.Reels?.FirstOrDefault();

        if (firstReel != null)
        {
            SelectReel(firstReel);
        }
    }

    private async Task<List<Product>> FetchVideosFromApi(int currentBatch, int batchSize)
    {
        int page = currentBatch;
        int pageSize = batchSize;
        var url = $"{baseUrl}/paginatedApproved/?page={page}&recordsnumber={pageSize}";

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

    //private void UpdateDisplayVideos()
    //{
    //    currentDisplayVideos = allFetchedReels
    //        .Skip(CurrentBatch * DisplaySize)
    //        .Take(DisplaySize)
    //        .ToList();
    //}

    //public async Task LoadNextBatch()
    //{
    //    Console.WriteLine("INGRESA");
    //    currentVideoIndex++;
    //    Console.WriteLine(currentBatch + 1);
    //    Console.WriteLine((currentBatch + 1) * DisplaySize);
    //    Console.WriteLine(allFetchedReels.Count);
    //    if ((currentBatch + 1) * DisplaySize >= allFetchedReels.Count)
    //    {
    //        Console.WriteLine("LLAMADO");
    //        Console.WriteLine((currentBatch + 1) * DisplaySize >= allFetchedReels.Count);
    //        currentVideoIndex = 0;
    //        await FetchVideoBatch(); // Carga nuevos 10 videos si es necesario
    //    }
    //    Console.WriteLine("CONTU");
    //    currentBatch++;
    //    await SelectVideo(currentVideoIndex);
    //    Console.WriteLine("ACTUA");
    //    UpdateDisplayVideos();
    //}
    public async Task LoadNextBatch()
    {
        // Valida si no esta en el último video para la reproduccion automatica
        if (IsOnLastVideo) return;

        // 1. Avanzar al siguiente índice de video
        CurrentVideoIndex++;

        // 3. Verificar si necesitamos cargar más videos del backend
        bool needsMoreVideos = CurrentVideoIndex >= allFetchedReels.Count;

        // 4. Si estamos en el último video Y no hay más videos cargados
        if (needsMoreVideos)
        {
            allFetchedReels = await FetchVideosFromApi(++CurrentBatch, BatchSize);
            CurrentVideoIndex = 0;
        }
        // 6. Seleccionar el video actual
        await SelectVideo(CurrentVideoIndex);
    }

    public async Task LoadPreviousBatch()
    {
        // 1. Retroceder el índice
        CurrentVideoIndex--;

        // 2. Verificar si necesitamos cargar el batch anterior
        bool isFirstVideoInBatch = CurrentVideoIndex < 0;
        bool hasPreviousBatches = CurrentBatch > 1;

        if (isFirstVideoInBatch)
        {
            allFetchedReels = await FetchVideosFromApi(--CurrentBatch, BatchSize);

            // 4. Cambiar al batch anterior
            CurrentVideoIndex = allFetchedReels.Count - 1; // Posición en el último video del batch anterior
        }
        // 6. Seleccionar el video actual
        await SelectVideo(CurrentVideoIndex);
    }

    private void SelectReel(Reel reel)
    {
        selectedReel = reel;
        StateHasChanged(); // Forzar actualización de la UI
    }

    public bool IsOnLastVideo
    {
        get
        {
            // Calcular el índice global del video actual
            return CurrentBatch == totalPages && (CurrentVideoIndex + 1) == allFetchedReels.Count;
        }
    }

    private bool HasPreviousBatchLoaded
    {
        get
        {
            // Verificar si es el último video
            return CurrentBatch <= 1 && CurrentVideoIndex <= 0;
        }
    }

    private void ToggleLike()
    {
    }

    private void ShowInfo()
    {
    }

    private void ShowComments()
    {
    }
}