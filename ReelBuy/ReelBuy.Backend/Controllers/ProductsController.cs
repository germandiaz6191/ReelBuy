using Microsoft.AspNetCore.Mvc;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;

namespace ReelBuy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : GenericController<Product>
{
    private readonly IProductsUnitOfWork _productsUnitOfWork;
    private readonly IFileStorage _fileStorage;
    private readonly IGenericUnitOfWork<Product> _unit;

    public ProductsController(IGenericUnitOfWork<Product> unit, IProductsUnitOfWork productsUnitOfWork, IFileStorage fileStorage) : base(unit)
    {
        _unit = unit;
        _productsUnitOfWork = productsUnitOfWork;
        _fileStorage = fileStorage;
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
    public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
    {
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
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
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
        if (model.Reels.Any())
        {
            foreach (var reel in model.Reels)
            {
                if (!string.IsNullOrEmpty(reel.ReelUri))
                {
                    var productReel = Convert.FromBase64String(reel.ReelUri);
                    reel.ReelUri = await _fileStorage.SaveFileAsync(productReel, ".mp4", "reels");
                }
            }
        }

        var action = await _unit.AddAsync(model);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }
}