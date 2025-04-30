using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;
using ReelBuy.Shared.Enums;

namespace ReelBuy.Frontend.Pages.Products;

public partial class ProductCreate
{
    private ProductForm? productForm;
    private Product product = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    protected override void OnInitialized()
    {
        product.StatusId = (int)StatusProduct.Pending;
    }

    private async Task CreateAsync()
    {
        var responseHttp = await Repository.PostAsync("/api/products", product);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        Return();
        Snackbar.Add(Localizer["RecordCreatedOk"], Severity.Success);
    }

    private void Return()
    {
        productForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("/products");
    }
}
