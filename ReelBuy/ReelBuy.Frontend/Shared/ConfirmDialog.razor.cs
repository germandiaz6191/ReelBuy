using ReelBuy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace ReelBuy.Frontend.Shared;

public partial class ConfirmDialog : ComponentBase
{
    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = null!;

    [Inject]
    private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Parameter]
    public string Message { get; set; } = null!;

    private void Accept()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}