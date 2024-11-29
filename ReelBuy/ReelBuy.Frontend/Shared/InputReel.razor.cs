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
    [Parameter] public string? ImageURL { get; set; }
    [Parameter] public EventCallback<string> ImageSelected { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (string.IsNullOrWhiteSpace(Label))
        {
            Label = Localizer["Reel"];
        }
    }

    private async Task OnChange(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file != null)
        {
            reelFileName = file.Name;

            var arrBytes = new byte[file.Size];
            await file.OpenReadStream().ReadAsync(arrBytes);
            reelBase64 = Convert.ToBase64String(arrBytes);
            ImageURL = null;
            await ImageSelected.InvokeAsync(reelBase64);
            StateHasChanged();
        }
    }
}