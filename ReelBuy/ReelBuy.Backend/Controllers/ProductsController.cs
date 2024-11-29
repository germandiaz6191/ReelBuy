using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;

namespace ReelBuy.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class ProductsController : GenericController<Product>
{
    private readonly IProductsUnitOfWork _productsUnitOfWork;

    public ProductsController(IGenericUnitOfWork<Product> unit, IProductsUnitOfWork productsUnitOfWork) : base(unit)
    {
        _productsUnitOfWork = productsUnitOfWork;
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
            return Ok(response.Result);
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
}