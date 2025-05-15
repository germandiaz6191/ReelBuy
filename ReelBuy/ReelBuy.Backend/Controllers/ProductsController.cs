using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Enums;

namespace ReelBuy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : GenericController<Product>
{
    private readonly IProductsUnitOfWork _productsUnitOfWork;
    private readonly IFileStorage _fileStorage;
    private readonly IGenericUnitOfWork<Product> _unit;
    private readonly IUsersUnitOfWork _usersUnitOfWork;
    private readonly IStatusesUnitOfWork _statusesUnitOfWork;
    private readonly ICategoriesUnitOfWork _categoriesUnitOfWork;
    private readonly IMarketplacesUnitOfWork _marketplacesUnitOfWork;
    private readonly IStoresUnitOfWork _storesUnitOfWork;

    public ProductsController(IGenericUnitOfWork<Product> unit, IProductsUnitOfWork productsUnitOfWork, IFileStorage fileStorage, IUsersUnitOfWork usersUnitOfWork, IStatusesUnitOfWork statusesUnitOfWork, ICategoriesUnitOfWork categoriesUnitOfWork, IMarketplacesUnitOfWork marketplacesUnitOfWork, IStoresUnitOfWork storesUnitOfWork) : base(unit)
    {
        _unit = unit;
        _productsUnitOfWork = productsUnitOfWork;
        _fileStorage = fileStorage;
        _usersUnitOfWork = usersUnitOfWork;
        _statusesUnitOfWork = statusesUnitOfWork;
        _categoriesUnitOfWork = categoriesUnitOfWork;
        _marketplacesUnitOfWork = marketplacesUnitOfWork;
        _storesUnitOfWork = storesUnitOfWork;
    }

    [HttpGet("combo")]
    public async Task<IActionResult> GetComboAsync()
    {
        return Ok(await _productsUnitOfWork.GetComboAsync());
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync()
    {
        var response = await _productsUnitOfWork.GetAsync();
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _productsUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            var reelUri = response.Result?.Reels?.FirstOrDefault()?.ReelUri;
            if (!string.IsNullOrEmpty(reelUri))
            {
                var pathUri = reelUri.Split("/").Last();
                var reelBytes = await _fileStorage.GetFileAsync(pathUri, "reels");
                var firstReel = response.Result?.Reels?.FirstOrDefault();
                if (firstReel != null)
                {
                    firstReel.ReelUri = Convert.ToBase64String(reelBytes);
                }
            }
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpGet("paginated")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
    {
        // Determinar si el usuario es administrador
        bool isAdmin = User.IsInRole("Admin");

        // Si no es administrador, filtrar solo por sus tiendas
        if (!isAdmin && !string.IsNullOrEmpty(pagination.StoreIds))
        {
            // La lógica de filtrado de tiendas se maneja en el frontend
            var response = await _productsUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                var currentReels = response.Result;
                if (currentReels == null)
                {
                    return Ok(currentReels);
                }
                foreach (Product reel in currentReels)
                {
                    var reelItem = reel.Reels.FirstOrDefault();
                    if (reelItem != null && !string.IsNullOrEmpty(reelItem.ReelUri))
                    {
                        var pathUri = reelItem.ReelUri.Split("/").Last();
                        var reelBytes = await _fileStorage.GetFileAsync(pathUri, "reels");

                        reelItem.ReelUri = Convert.ToBase64String(reelBytes);
                    }
                }

                return Ok(currentReels);
            }
            return BadRequest();
        }
        else if (isAdmin)
        {
            // Si es administrador, permitir ver todos los productos
            // Limpiar el filtro de tiendas para permitir ver todos los productos
            pagination.StoreIds = null;
            var response = await _productsUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                var currentReels = response.Result;
                if (currentReels == null)
                {
                    return Ok(currentReels);
                }
                foreach (Product reel in currentReels)
                {
                    var reelItem = reel.Reels.FirstOrDefault();
                    if (reelItem != null && !string.IsNullOrEmpty(reelItem.ReelUri))
                    {
                        var pathUri = reelItem.ReelUri.Split("/").Last();
                        var reelBytes = await _fileStorage.GetFileAsync(pathUri, "reels");

                        reelItem.ReelUri = Convert.ToBase64String(reelBytes);
                    }
                }

                return Ok(currentReels);
            }
            return BadRequest();
        }
        else
        {
            // Si no especificó tiendas y no es admin, no mostrar resultados
            return Ok(new List<Product>());
        }
    }

    [HttpGet("paginatedApproved")]
    public async Task<IActionResult> GetPaginateApprovedAsync([FromQuery] PaginationDTO pagination)
    {
        pagination.FilterStatus = (int)StatusProduct.Approved;
        var response = await _productsUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            var currentReels = response.Result;
            if (currentReels == null)
            {
                return Ok(currentReels);
            }
            foreach (Product reel in currentReels)
            {
                var reelItem = reel.Reels.FirstOrDefault();
                if (reelItem != null && !string.IsNullOrEmpty(reelItem.ReelUri))
                {
                    var pathUri = reelItem.ReelUri.Split("/").Last();
                    var reelBytes = await _fileStorage.GetFileAsync(pathUri, "reels");

                    reelItem.ReelUri = Convert.ToBase64String(reelBytes);
                }
            }

            return Ok(currentReels);
        }
        return BadRequest();
    }

    [HttpGet("totalRecordsPaginated")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        // Determinar si el usuario es administrador
        bool isAdmin = User.IsInRole("Admin");

        // Si es administrador, permitir ver el total de todos los productos
        if (isAdmin)
        {
            pagination.StoreIds = null; // Limpiar filtro de tiendas para ver todo
        }

        var action = await _productsUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecordsPaginatedApproved")]
    public async Task<IActionResult> GetTotalRecordsApprovedAsync([FromQuery] PaginationDTO pagination)
    {
        pagination.FilterStatus = (int)StatusProduct.Approved;
        var action = await _productsUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Product>>> SearchProducts([FromQuery] PrincipalSearchDTO principalSearch)
    {
        var action = await _productsUnitOfWork.GetProductsByLikeAsync(principalSearch);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpPost("CreateProduct")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateProduct([FromBody] Product model)
    {
        var ResponseStatus = await _statusesUnitOfWork.GetAsync(model.StatusId);
        if (!ResponseStatus.WasSuccess || ResponseStatus.Result == null)
        {
            return NotFound("Estado no encontrado");
        }

        var ResponseCategory = await _categoriesUnitOfWork.GetAsync(model.CategoryId);
        if (!ResponseCategory.WasSuccess || ResponseCategory.Result == null)
        {
            return NotFound("Categoría no encontrada");
        }

        var ResponseMarketplace = await _marketplacesUnitOfWork.GetAsync(model.MarketplaceId);
        if (!ResponseMarketplace.WasSuccess || ResponseMarketplace.Result == null)
        {
            return NotFound("Marketplace no encontrada");
        }

        var ResponseStore = await _storesUnitOfWork.GetAsync(model.StoreId);
        if (!ResponseStore.WasSuccess || ResponseStore.Result == null)
        {
            return NotFound("Store no encontrado");
        }

        if (model.Reels.Any())
        {
            foreach (var reel in model.Reels)
            {
                if (!string.IsNullOrEmpty(reel.ReelUri))
                {
                    var productReel = Convert.FromBase64String(reel.ReelUri);
                    reel.ReelUri = await _fileStorage.SaveFileAsync(productReel, ".mp4", "reels");
                    //reel.ReelUri = "https://azurerb2025.blob.core.windows.net/reels/15613a15-bb46-417c-90ad-d456853a2874.mp4";
                }
            }
        }

        model.Store = ResponseStore.Result;
        model.Status = ResponseStatus.Result;
        model.Category = ResponseCategory.Result;

        var action = await _unit.AddAsync(model);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpPut("statuses")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> UpdateProducts([FromBody] List<Product> models)
    {
        var action = await _productsUnitOfWork.UpdateAsync(models);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpPut("UpdateProduct")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> PutAsyncUpdate(ProductDTO model)
    {
        // Verificar si el usuario es administrador o si el producto pertenece a una de sus tiendas
        bool isAdmin = User.IsInRole("Admin");
        var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
        string userId = currentUser.Id;

        // Obtener el producto actual para verificar a qué tienda pertenece
        var Response = await _productsUnitOfWork.GetAsync(model.Id);
        if (!Response.WasSuccess || Response.Result == null)
        {
            return NotFound("Producto no encontrado");
        }

        Product CurrentProduct = Response.Result;

        if (!isAdmin && CurrentProduct.Store?.UserId != userId)// Verificar si la tienda pertenece al usuario
        {
            return Forbid("No tienes permiso para editar este producto");
        }

        var ResponseStatus = await _statusesUnitOfWork.GetAsync(model.StatusId);
        if (!ResponseStatus.WasSuccess || ResponseStatus.Result == null)
        {
            return NotFound("Estado no encontrado");
        }

        var ResponseCategory = await _categoriesUnitOfWork.GetAsync(model.CategoryId);
        if (!ResponseCategory.WasSuccess || ResponseCategory.Result == null)
        {
            return NotFound("Categoría no encontrada");
        }

        var ResponseMarketplace = await _marketplacesUnitOfWork.GetAsync(model.MarketplaceId);
        if (!ResponseMarketplace.WasSuccess || ResponseMarketplace.Result == null)
        {
            return NotFound("Marketplace no encontrada");
        }

        var ResponseStore = await _storesUnitOfWork.GetAsync(model.StoreId);
        if (!ResponseStore.WasSuccess || ResponseStore.Result == null)
        {
            return NotFound("Store no encontrado");
        }

        CurrentProduct.Name = model.Name;
        CurrentProduct.Description = model.Description;
        CurrentProduct.Price = model.Price;
        CurrentProduct.PathUri = model.PathUri;
        CurrentProduct.Store = ResponseStore.Result;
        CurrentProduct.Status = ResponseStatus.Result;
        CurrentProduct.Category = ResponseCategory.Result;

        if (model.Reels != null && model.Reels.Any())
        {
            foreach (var reel in model.Reels)
            {
                if (!string.IsNullOrEmpty(reel.ReelUri) && !reel.ReelUri.Contains(".mp4"))
                {
                    var productReel = Convert.FromBase64String(reel.ReelUri);
                    reel.ReelUri = await _fileStorage.SaveFileAsync(productReel, ".mp4", "reels");
                    //reel.ReelUri = "https://azurerb2025.blob.core.windows.net/reels/15613a15-bb46-417c-90ad-d456853a2874.mp4";
                }
            }
            model.Reels = CurrentProduct.Reels;
        }

        var response = await base.PutAsync(CurrentProduct);
        if (response is OkObjectResult)
        {
            return response;
        }
        return BadRequest();
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public override async Task<IActionResult> DeleteAsync(int id)
    {
        // Verificar si el usuario es administrador o si el producto pertenece a una de sus tiendas
        bool isAdmin = User.IsInRole("Admin");
        var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
        string userId = currentUser.Id;

        if (!isAdmin)
        {
            // Obtener el producto actual para verificar a qué tienda pertenece
            var currentProduct = await _productsUnitOfWork.GetAsync(id);
            if (!currentProduct.WasSuccess || currentProduct.Result == null)
            {
                return NotFound("Producto no encontrado");
            }

            // Verificar si la tienda pertenece al usuario
            if (currentProduct.Result.Store?.UserId != userId)
            {
                return Forbid("No tienes permiso para eliminar este producto");
            }
        }

        var response = await base.DeleteAsync(id);
        if (response is NoContentResult)
        {
            return response;
        }
        return BadRequest();
    }
}