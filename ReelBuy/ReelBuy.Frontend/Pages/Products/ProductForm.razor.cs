using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ReelBuy.Shared.Resources;
using ReelBuy.Shared.Entities;
using ReelBuy.Frontend.Repositories;
using MudBlazor;
using ReelBuy.Shared.Enums;
using ReelBuy.Frontend.Shared;

namespace ReelBuy.Frontend.Pages.Products;

public partial class ProductForm
{
    private EditContext editContext = null!;
    private Category selectedCategory = new();
    private List<Category>? categories;
    private Marketplace selectedMarketplace = new();
    private List<Marketplace>? marketplaces;
    private Product product = new();
    private Store selectedStore = new();
    private List<Store>? stores;
    private List<Status>? statuses;
    private string? reelBase64;
    private string? reelUrl;
    private RichTextEditor? richTextEditor;


    // Propiedades para manejar el estado del producto
    private bool IsStatusPending => Product.StatusId == (int)StatusProduct.Pending;
    private bool IsStatusApproved => Product.StatusId == (int)StatusProduct.Approved;
    private bool IsStatusInactive => Product.StatusId == (int)StatusProduct.Inactive;
    private bool IsStatusReject => Product.StatusId == (int)StatusProduct.Reject;
    
    private bool ShowStatusSelect => !IsStatusPending && !IsStatusInactive && !IsStatusReject;
    private bool ShowActivateButton => IsStatusInactive;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [EditorRequired, Parameter] public Product Product { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }
    [Parameter] public EventCallback SaveCallback { get; set; }
    [Parameter] public bool ShowMessage { get; set; } = true;

    protected override void OnInitialized()
    {
        editContext = new(Product);
        product = Product;
        
        // Asegúrese de que la descripción tenga un valor significativo
        if (string.IsNullOrEmpty(product.Description) || product.Description == "<p>&nbsp;</p>")
        {
            product.Description = "";
        }
    }

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine($"ProductForm.OnInitializedAsync - Product.Id: {Product.Id}, Product.Description: {(Product.Description?.Length > 10 ? Product.Description?.Substring(0, 10) + "..." : Product.Description)}");
        
        await LoadCategoriesAsync();
        await LoadMarketplacesAsync();
        await LoadStoresAsync();
        await LoadStatusesAsync();
        
        if (Product.Id == 0)
        {
            Product.StatusId = (int)StatusProduct.Pending;
        }
        
        editContext = new EditContext(Product);
        
        if (Product.CategoryId > 0 && categories != null)
        {
            selectedCategory = categories.FirstOrDefault(c => c.Id == Product.CategoryId) ?? new Category();
        }
        
        if (Product.MarketplaceId > 0 && marketplaces != null)
        {
            selectedMarketplace = marketplaces.FirstOrDefault(c => c.Id == Product.MarketplaceId) ?? new Marketplace();
        }
        
        if (Product.StoreId > 0 && stores != null)
        {
            selectedStore = stores.FirstOrDefault(c => c.Id == Product.StoreId) ?? new Store();
        }
    }

    private async Task LoadStatusesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Status>>("api/statuses");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        statuses = responseHttp.Response;
        StateHasChanged();
    }

    private void CategoryChanged(Category category)
    {
        selectedCategory = category;
    }

    private void MarketplaceChanged(Marketplace marketplace)
    {
        selectedMarketplace = marketplace;
    }

    private void StoreChanged(Store store)
    {
        selectedStore = store;
    }

    private Func<string, CancellationToken, Task<IEnumerable<Category>>> SearchCategories => async (searchText, cancellationToken) =>
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return categories!;
        }

        return categories!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
        .ToList();
    };

    private Func<string, CancellationToken, Task<IEnumerable<Marketplace>>> SearchMarketplaces => async (searchText, cancellationToken) =>
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return marketplaces!;
        }

        return marketplaces!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    };

    private Func<string, CancellationToken, Task<IEnumerable<Store>>> SearchStores => async (searchText, cancellationToken) =>
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return stores!;
        }

        return stores!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    };

    private async Task LoadCategoriesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Category>>("/api/categories/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        categories = responseHttp.Response;
    }

    private async Task LoadMarketplacesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Marketplace>>("/api/marketplaces/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        marketplaces = responseHttp.Response;
    }

    private async Task LoadStoresAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Store>>("/api/stores/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        stores = responseHttp.Response;
    }

    // Método que responde a cambios en la descripción desde el editor
    private void OnDescriptionChanged(string newContent)
    {
        Console.WriteLine($"OnDescriptionChanged - Nuevo contenido: {(newContent?.Length > 20 ? newContent?.Substring(0, 20) + "..." : newContent)}");
        
        // No guardar el contenido si es sólo un espacio en blanco
        if (newContent != "<p>&nbsp;</p>")
        {
            Product.Description = newContent;
        }
        else
        {
            Product.Description = "";
        }
        
        editContext?.Validate();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Product.Id > 0)
        {
            Console.WriteLine($"ProductForm.OnAfterRenderAsync - firstRender: {firstRender}, Product.Id: {Product.Id}");
            Console.WriteLine($"ProductForm.OnAfterRenderAsync - Description: {(Product.Description?.Length > 10 ? Product.Description?.Substring(0, 10) + "..." : Product.Description)}");
            
            // Si el producto ya tiene un ID, aseguramos que la descripción se carga en el editor
            if (richTextEditor != null && !string.IsNullOrWhiteSpace(Product.Description))
            {
                // Dar más tiempo para que el editor y su elemento DOM se inicialice completamente
                await Task.Delay(800);
                
                try
                {
                    await richTextEditor.SetHTML(Product.Description);
                    Console.WriteLine("ProductForm.OnAfterRenderAsync - Se ha establecido el HTML en el editor");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ProductForm.OnAfterRenderAsync - Error al establecer HTML: {ex.Message}");
                    
                    // Intentar nuevamente después de un tiempo adicional
                    await Task.Delay(500);
                    try
                    {
                        await richTextEditor.SetHTML(Product.Description);
                        Console.WriteLine("ProductForm.OnAfterRenderAsync - Se ha establecido el HTML en el segundo intento");
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine($"ProductForm.OnAfterRenderAsync - Error en segundo intento: {ex2.Message}");
                    }
                }
            }
        }
    }

    private async Task SaveProductAsync()
    {
        try
        {
            Console.WriteLine("SaveProductAsync iniciado");
            
            // Obtener el contenido HTML del editor y asignarlo a la descripción del producto
            string editorContent = "";
            
            try
            {
                if (richTextEditor != null)
                {
                    editorContent = await richTextEditor.GetHTML();
                    Console.WriteLine($"Contenido obtenido del editor: {(editorContent?.Length > 10 ? editorContent?.Substring(0, 10) + "..." : editorContent)}");
                }
                else 
                {
                    Console.WriteLine("richTextEditor es null");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener HTML del editor: {ex.Message}");
                // Continuar con el proceso de guardado incluso si falla la obtención del contenido
            }
            
            // Verificar si el contenido es significativo (no solo un espacio en blanco)
            if (!string.IsNullOrWhiteSpace(editorContent) && 
                editorContent != "<p>&nbsp;</p>" && 
                editorContent != "<p></p>")
            {
                Product.Description = editorContent;
            }
            else 
            {
                Console.WriteLine("Editor content is empty or just whitespace");
                Product.Description = ""; // Guardar como cadena vacía en lugar de espacio en blanco
            }
            
            Console.WriteLine($"Category ID: {selectedCategory?.Id}, Marketplace ID: {selectedMarketplace?.Id}, Store ID: {selectedStore?.Id}");
            
            // Asignar IDs de relaciones
            Product.CategoryId = selectedCategory?.Id ?? 0;
            Product.MarketplaceId = selectedMarketplace?.Id ?? 0;
            Product.StoreId = selectedStore?.Id ?? 0;
            
            // Antes de guardar
            Console.WriteLine($"Guardando producto. Id: {Product.Id}, Nombre: {Product.Name}, Descripción: {(Product.Description?.Length > 10 ? Product.Description?.Substring(0, 10) + "..." : Product.Description)}");
            
            HttpResponseWrapper<object> response;
            if (Product.Id == 0)
            {
                response = await Repository.PostAsync("api/products", Product);
            }
            else
            {
                response = await Repository.PutAsync($"api/products/{Product.Id}", Product);
            }
            
            bool result = !response.Error;
            Console.WriteLine($"Resultado del guardado: {result}");
            
            if (result)
            {
                if (ShowMessage)
                {
                    NavigationManager.NavigateTo($"/products/details/{Product.Id}");
                    Snackbar.Add(Localizer["ChangesSaved"], Severity.Success);
                }
                
                await SaveCallback.InvokeAsync();
            }
            else
            {
                var errorMessage = await response.GetErrorMessageAsync();
                Snackbar.Add(errorMessage ?? Localizer["ChangesNotSaved"], Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en SaveProductAsync: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private void Return()
    {
        FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("/products");
    }

    public bool FormPostedSuccessfully { get; set; } = false;

    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formWasEdited = editContext.IsModified();

        if (!formWasEdited || FormPostedSuccessfully)
        {
            return;
        }

        var result = await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"],
            Text = Localizer["LeaveAndLoseChanges"],
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true
        });

        var confirm = !string.IsNullOrEmpty(result.Value);
        if (confirm)
        {
            return;
        }

        context.PreventNavigation();
    }

    private void ReelSelected(string _reelUri)
    {
        reelUrl = _reelUri;
    }

    private Reel CreateReel()
    {
        var reel = new Reel
        {
            Name = product.Name + "-reel-product",
            ProductId = product!.Id,
            ReelUri = reelUrl!
        };
        return reel;
    }

    private async Task ActivateProductAsync()
    {
        Product.StatusId = (int)StatusProduct.Pending;
        await SaveProductAsync();
    }
}