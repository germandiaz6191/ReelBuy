using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages;

public partial class Home
{
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
}