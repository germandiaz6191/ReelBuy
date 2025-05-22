namespace ReelBuy.Frontend.Shared;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ReelBuy.Shared.Resources;

public partial class InputReel
{
    private string? reelBase64;
    private string? reelFileName;

    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Parameter] public string? Label { get; set; }
    [Parameter] public string? ReelURL { get; set; }
    [Parameter] public string? ReelBase64 { get; set; }
    [Parameter] public EventCallback<string> ReelSelected { get; set; }

    const long MaxFileSize = 100 * 1024 * 1024; // 10 MB

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (string.IsNullOrWhiteSpace(Label))
        {
            Label = Localizer["Reel"];
        }
        if (!string.IsNullOrEmpty(ReelBase64))
        {
            reelBase64 = ReelBase64;
            ReelURL = null;
        }
    }

    private async Task OnChange(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file.Size > MaxFileSize)
        {
            Console.WriteLine("El archivo es demasiado grande.");
            return;
        }
        if (file != null)
        {
            reelFileName = file.Name;

            var arrBytes = new byte[file.Size];
            await file.OpenReadStream(MaxFileSize).ReadAsync(arrBytes);
            reelBase64 = Convert.ToBase64String(arrBytes);
            ReelURL = null;
            await ReelSelected.InvokeAsync(reelBase64);
            StateHasChanged();
        }
    }
}