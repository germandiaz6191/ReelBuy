using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.Reels;

public partial class ReelEdit
{
    private Reel? reel;
    private ReelForm? reelForm;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<Reel>($"api/reels/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("reels");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            reel = responseHttp.Response;
        }
    }

    [Authorize(Roles = "Admin,Seller")]
    private async Task EditAsync()
    {
        var responseHttp = await Repository.PutAsync("api/reels", reel);

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        Return();
        Snackbar.Add(Localizer["RecordSavedOk"], Severity.Success);
    }

    private void Return()
    {
        reelForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("reels");
    }
}