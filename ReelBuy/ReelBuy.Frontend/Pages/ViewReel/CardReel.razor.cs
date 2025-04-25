using Microsoft.AspNetCore.Components;
using ReelBuy.Shared.Entities;

namespace ReelBuy.Frontend.Pages.ViewReel;

public partial class CardReel
{
    [Parameter] public Reel? ReelData { get; set; }
    [Parameter] public EventCallback OnNext { get; set; }
    [Parameter] public EventCallback OnPrevious { get; set; }
    [Parameter] public EventCallback OnLike { get; set; }
    [Parameter] public EventCallback OnInfo { get; set; }
    [Parameter] public bool IsNextDisabled { get; set; } = false;
    [Parameter] public bool IsPreviousDisabled { get; set; } = false;
}