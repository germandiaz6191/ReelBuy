using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Shared;

public partial class GenericList<Titem>
{
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Parameter]
    public RenderFragment? Loading { get; set; }

    [Parameter]
    public RenderFragment? NoRecords { get; set; }

    [EditorRequired]
    [Parameter]
    public RenderFragment Body { get; set; } = null!;

    [EditorRequired]
    [Parameter]
    public List<Titem> MyList { get; set; } = null!;
}