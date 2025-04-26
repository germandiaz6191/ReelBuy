using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.ViewReel;

public partial class CardReel
{
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Parameter] public Reel? ReelData { get; set; }
    [Parameter] public EventCallback OnNext { get; set; }
    [Parameter] public EventCallback OnPrevious { get; set; }
    [Parameter] public EventCallback OnLike { get; set; }
    [Parameter] public EventCallback OnInfo { get; set; }
    [Parameter] public bool IsNextDisabled { get; set; } = false;
    [Parameter] public bool IsPreviousDisabled { get; set; } = false;
}