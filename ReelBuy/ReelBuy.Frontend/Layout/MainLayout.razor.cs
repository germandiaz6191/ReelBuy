using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Layout;

public partial class MainLayout
{
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
}