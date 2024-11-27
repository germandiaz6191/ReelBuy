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
public class StoresController : GenericController<Store>
{
    private readonly IStoresUnitOfWork _storesUnitOfWork;

    public StoresController(IGenericUnitOfWork<Store> unit, IStoresUnitOfWork storesUnitOfWork) : base(unit)
    {
        _storesUnitOfWork = storesUnitOfWork;
    }

    [HttpGet("combo")]
    public async Task<IActionResult> GetComboAsync()
    {
        return Ok(await _storesUnitOfWork.GetComboAsync());
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync()
    {
        var response = await _storesUnitOfWork.GetAsync();
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _storesUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
    {
        var response = await _storesUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecordsPaginated")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _storesUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpPost("full")]
    public async Task<IActionResult> PostAsync(StoreDTO storeDTO)
    {
        var action = await _storesUnitOfWork.AddAsync(storeDTO);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpPut("full")]
    public async Task<IActionResult> PutAsync(StoreDTO storeDTO)
    {
        var action = await _storesUnitOfWork.UpdateAsync(storeDTO);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }
}