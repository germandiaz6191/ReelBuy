using Microsoft.AspNetCore.Components;

namespace ReelBuy.Frontend.Pages.CardReel;

public partial class CardReel
{
    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string Thumbnail { get; set; }

    [Parameter]
    public string Link { get; set; }
}