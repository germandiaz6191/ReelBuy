using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.ViewReel;

public partial class CardReel
{
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
    [Parameter] public Product ReelData { get; set; } = null!;
    [Parameter] public EventCallback OnNext { get; set; }
    [Parameter] public EventCallback OnPrevious { get; set; }
    [Parameter] public bool IsNextDisabled { get; set; } = false;
    [Parameter] public bool IsPreviousDisabled { get; set; } = false;
    [Parameter, SupplyParameterFromQuery] public string? Filter { get; set; } = null;
    protected Reel? FirstReel => ReelData?.Reels?.FirstOrDefault();

    private Breakpoint _breakpoint;
    private bool IsMobile => _breakpoint < Breakpoint.Md;
    private string DrawerWidth => IsMobile ? "100vw" : "350px";
    private string DrawerClass => IsMobile ? "p-4" : "p-3";

    private const string baseUrlLikes = "api/likes";
    private const string baseUrlFavorite = "api/favorites";
    private const string baseUrlComments = "api/comments";
    private bool IsLiked = false;
    private bool IsFavorite = false;
    private Favorite? favorite;
    private bool IsAuthenticate = false;
    private string? Identity = string.Empty;
    private bool ShowComments { get; set; } = false;
    private string NewComment { get; set; } = string.Empty;
    private int? ConfirmingCommentId { get; set; }
    private const int PageSize = 100;
    private int Page = 0;
    private bool _loadingMore = false;
    private bool _showMoreComments = true;

    private List<Comments> Comments = new();

    protected override async Task OnParametersSetAsync()
    {
        if (ReelData == null) { return; }
        Comments = new();
        await LoadUserAsync();

        await Task.WhenAll(LoadFavoriteByUserAsync(), LoadLikeByUserAsync());
    }

    private void OnBreakpointChanged(Breakpoint breakpoint)
    {
        _breakpoint = breakpoint;
    }

    private string FormatNumber(int number)
    {
        if (number >= 1_000_000)
            return (number / 1_000_000D).ToString("0.#") + "M";
        if (number >= 1_000)
            return (number / 1_000D).ToString("0.#") + "k";
        return number.ToString();
    }

    private async Task LoadUserAsync()
    {
        if (AuthenticationStateProvider == null)
        {
            return;
        }
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
        {
            IsAuthenticate = true;
            Identity = user.FindFirst(c => c.Type == "UserId")?.Value;
        }
    }

    private async Task LoadLikeByUserAsync()
    {
        if (!IsAuthenticate)
        {
            return;
        }

        var responseHttp = await Repository.GetAsync<bool>($"{baseUrlLikes}/{Identity}/{ReelData?.Id}");
        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }
        IsLiked = responseHttp.Response;
    }

    private async Task LoadFavoriteByUserAsync()
    {
        if (!IsAuthenticate)
        {
            return;
        }

        var responseHttp = await Repository.GetAsync<Favorite>($"{baseUrlFavorite}/{Identity}/{ReelData?.Id}");

        if (responseHttp.Error || responseHttp.Response == null)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }
        favorite = responseHttp.Response;
        IsFavorite = responseHttp?.Response.Id != 0;
    }

    private async Task OnInfoAsync(string? Description)
    {
        if (string.IsNullOrEmpty(Description))
        {
            return;
        }

        // Crea y muestra el diálogo
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };

        var parameters = new DialogParameters
        {
            { "Description", Description }
        };

        var dialog = DialogService.Show<RedirectDialog>("Confirmar redirección", parameters, options);
        var result = await dialog.Result;

        // Si el usuario confirma, redirige
        if (result != null && !result.Canceled)
        {
            //if (Uri.IsWellFormedUriString(Description, UriKind.Absolute))
            // NavigationManager.NavigateTo(Description, forceLoad: true);
        }
    }

    private async Task OnBuyAsync(Product product)
    {
        var uriProduct = product.Marketplace?.Domain?.ToString();
        // Crea y muestra el diálogo
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };

        var parameters = new DialogParameters
        {
            { "Description", Localizer["DialogBuyDescription"].Value },
            { "DialigFirstButton", Localizer["DialigFirstButton"].Value },
            { "DialigSecondButton", Localizer["DialigSecondButton"].Value },
            { "DialigShowButton", true },
            { "DialigShowIcon", true }
        };

        var dialog = DialogService.Show<RedirectDialog>(Localizer["DialogBuyTitle"], parameters, options);
        var result = await dialog.Result;

        // Si el usuario confirma, redirige
        if (result != null && !result.Canceled)
        {
            await JSRuntime.InvokeVoidAsync("window.open", $"https://{uriProduct}", "_blank");
        }
    }

    private async Task OnLikeAsync(Product product)
    {
        if (!IsAuthenticate)
        {
            var message = Localizer["RequiredAuthentication"];
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        if (IsLiked)
        {
            await DeleteLikeAsync(product);
        }
        else
        {
            await AddLikeAsync(product);
        }
        IsLiked = !IsLiked;
    }

    private async Task OnFavoriteAsync(Product product)
    {
        if (!IsAuthenticate)
        {
            var message = Localizer["RequiredAuthentication"];
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        if (IsFavorite)
        {
            await DeleteFavoriteAsync();
        }
        else
        {
            await AddFavoriteAsync(product);
        }
        IsFavorite = !IsFavorite;
    }

    private async Task DeleteLikeAsync(Product product)
    {
        if (product == null)
        {
            var message = Localizer["GeneralError"];
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        var responseHttp = await Repository.DeleteAsync($"{baseUrlLikes}/DeleteLikeAsync/{Identity}/{product?.Id}");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);

            return;
        }
        product.LikesGroup--;
    }

    private async Task AddLikeAsync(Product product)
    {
        if (Identity == null)
        {
            var message = Localizer["GeneralError"];
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        LikeDTO addLike = new LikeDTO { ProductId = product.Id, UserId = Identity };
        var responseHttp = await Repository.PostAsync($"{baseUrlLikes}/PostLikeAsync", addLike);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);

            return;
        }
        product.LikesGroup++;
    }

    private async Task DeleteFavoriteAsync()
    {
        var foundFavorite = favorite?.Id;

        var responseHttp = await Repository.DeleteAsync($"{baseUrlFavorite}/{foundFavorite}");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);

            return;
        }
    }

    private async Task AddFavoriteAsync(Product product)
    {
        if (Identity == null)
        {
            var message = Localizer["FavoriteErrorUserNull"];
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        FavoriteDTO addFavorite = new FavoriteDTO { ProductId = product.Id, UserId = Identity };
        var responseHttp = await Repository.PostAsync<FavoriteDTO, Favorite>($"{baseUrlFavorite}/full", addFavorite);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);

            return;
        }

        favorite = responseHttp.Response;
    }

    private async Task OnInfoAsync(Product product)
    {
        var description = product.Description;
        //description = "<p><strong>Precio especial:</strong> $9.99</p><p>Envío gratis por tiempo limitado.</p>";
        // Crea y muestra el diálogo
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, MaxWidth = MaxWidth.ExtraLarge, };

        var parameters = new DialogParameters
        {
            { "Description", description },
            { "DialigFirstButton", Localizer["DialigFirstButton"].Value },
            { "DialigSecondButton", Localizer["DialigSecondButton"].Value },
            { "DialigShowButton", false },
            { "DialigShowIcon", false }
        };

        var dialog = DialogService.Show<RedirectDialog>(Localizer["DialogDescriptionTitle"], parameters, options);
        var result = await dialog.Result;
    }

    private async Task OnCommentsAsync()
    {
        ShowComments = !ShowComments;
        if (ShowComments)
        {
            Comments = new();
            Page = 0;
            await GetComments(ReelData, Page);
        }
    }

    private async Task LoadMoreCommentsAsync()
    {
        if (_loadingMore || !_showMoreComments) return;

        _loadingMore = true;

        await GetComments(ReelData, Page);

        _loadingMore = false;
    }

    private async Task GetComments(Product product, int page)
    {
        int currentPage = page + 1;
        Filter = product.Id.ToString();
        var url = $"{baseUrlComments}/paginatedByProduct/?page={currentPage}&recordsnumber={PageSize}&filter={Filter}";
        
        var responseHttp = await Repository.GetAsync<List<Comments>>(url);
        if (responseHttp.Response == null || responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        
        var newComments = responseHttp.Response;
        Page = currentPage;
        if (newComments?.Any() == true)
        {
            Comments.AddRange(newComments);
        }
        else
        {
            _showMoreComments = false;
        }
    }

    private async Task SendCommentAsync()
    {
        if (Identity == null)
        {
            var message = Localizer["GeneralError"];
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        CommetDTO addComment = new CommetDTO
        {
            ProductId = ReelData.Id,
            UserId = Identity,
            Description = NewComment,
            RegistrationDate = DateTime.UtcNow,
        };

        var responseHttp = await Repository.PostAsync<CommetDTO, Comments>($"{baseUrlComments}/full", addComment);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);

            return;
        }

        var getComment = responseHttp.Response;
        if (getComment != null)
        {
            NewComment = string.Empty;
            Comments.Add(getComment);
        }
    }

    private void CancelDelete()
    {
        ConfirmingCommentId = null;
    }

    private async Task OnDeleteCommentAsync(Comments comment)
    {
        var responseHttp = await Repository.DeleteAsync($"{baseUrlComments}/{comment.Id}");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        Comments.Remove(comment);
    }
}