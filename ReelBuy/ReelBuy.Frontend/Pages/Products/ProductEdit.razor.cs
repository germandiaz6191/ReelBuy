using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ReelBuy.Frontend.Repositories;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;

namespace ReelBuy.Frontend.Pages.Products;

public partial class ProductEdit
{
    private Product? product;
    private ProductForm? productForm;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<Product>($"api/products/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("products");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(messageError!, Severity.Error);
            }
        }
        else
        {
            product = responseHttp.Response;
        }
    }

    private async Task EditAsync()
    {
        // El formulario ya maneja la limpieza de entidades relacionadas
        // Asegurarnos de que el producto tenga el Id correcto
        product!.Id = Id;
        
        // Enviar la solicitud de actualización al backend
        var responseHttp = await Repository.PutAsync($"api/products/{Id}", product);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        
        Return();
        Snackbar.Add(Localizer["RecordUpdatedOk"], Severity.Success);
    }

    private void Return()
    {
        productForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("products");
    }
}