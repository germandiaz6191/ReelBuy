using ReelBuy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using MudBlazor.Dialog;

namespace ReelBuy.Frontend.Shared;

public partial class ConfirmDialog : ComponentBase
{
    [CascadingParameter] 
    MudDialogInstance MudDialog { get; set; } = null!;
    
    [Inject] 
    IStringLocalizer<Literals> Localizer { get; set; } = null!;
    
    [Parameter] 
    public string Message { get; set; } = null!;

    void Accept()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }

    void Cancel()
    {
        MudDialog.Cancel();
    }
}
