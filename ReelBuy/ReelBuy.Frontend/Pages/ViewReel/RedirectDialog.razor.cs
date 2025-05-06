using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ReelBuy.Frontend.Pages.ViewReel;

public partial class RedirectDialog
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
    [Parameter] public string Description { get; set; } = string.Empty;
    [Parameter] public string DialigFirstButton { get; set; } = null!;
    [Parameter] public string DialigSecondButton { get; set; } = null!;
    [Parameter] public bool DialigShowButton { get; set; } = false;
    [Parameter] public bool DialigShowIcon { get; set; } = false;
}