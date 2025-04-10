using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Frontend.Services;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.Auth;

public partial class Login
{
    private LoginDTO loginDTO = new();
    private bool wasClose;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ILoginService LoginService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

    private void CloseModal()
    {
        wasClose = true;
        MudDialog.Cancel();
    }

    private async Task LoginAsync()
    {
        if (wasClose)
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        var responseHttp = await Repository.PostAsync<LoginDTO, TokenDTO>("/api/accounts/Login", loginDTO);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        if (!string.IsNullOrEmpty(responseHttp.Response!.Token))
        {
            var userInformation = LoginService.ParseClaimsFromJWT(responseHttp.Response!.Token);
            var userID = userInformation.FirstOrDefault(x => x.Type == "UserId")?.Value;
            await JS.InvokeVoidAsync("localStorage.setItem", "UserId", userID);
        }

        await LoginService.LoginAsync(responseHttp.Response!.Token);
        NavigationManager.NavigateTo("/");
    }

    private void ShowModalResendConfirmationEmail()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true, MaxWidth = MaxWidth.ExtraLarge };
        DialogService.Show<ResendConfirmationEmailToken>(Localizer["MailForwarding"], closeOnEscapeKey);
    }

    private void ShowModalRecoverPassword()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraLarge };
        DialogService.Show<RecoverPassword>(Localizer["PasswordRecovery"], closeOnEscapeKey);
    }

}