using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;

namespace ReelBuy.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class FavoritesController : GenericController<Favorite>
{
    private readonly IFavoritesUnitOfWork _favoritesUnitOfWork;

    public FavoritesController(IGenericUnitOfWork<Favorite> unit, IFavoritesUnitOfWork favoritesUnitOfWork) : base(unit)
    {
        _favoritesUnitOfWork = favoritesUnitOfWork;
    }

    [HttpGet("combo")]
    public async Task<IActionResult> GetComboAsync()
    {
        return Ok(await _favoritesUnitOfWork.GetComboAsync());
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync()
    {
        var response = await _favoritesUnitOfWork.GetAsync();
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _favoritesUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpGet("{userId}/{productId}")]
    public async Task<IActionResult> GetFavoriteAsync(string userId, int productId)
    {
        var action = await _favoritesUnitOfWork.GetAsync(userId, productId);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
    {
        var response = await _favoritesUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecordsPaginated")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _favoritesUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpPost("full")]
    public async Task<IActionResult> PostAsync(FavoriteDTO favoriteDTO)
    {
        var action = await _favoritesUnitOfWork.AddAsync(favoriteDTO);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }
}