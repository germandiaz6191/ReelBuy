using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;

namespace ReelBuy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReelsController : GenericController<Reel>
{
    private readonly IReelsUnitOfWork _reelsUnitOfWork;

    public ReelsController(IGenericUnitOfWork<Reel> unit, IReelsUnitOfWork reelsUnitOfWork) : base(unit)
    {
        _reelsUnitOfWork = reelsUnitOfWork;
    }

    [HttpGet("combo")]
    public async Task<IActionResult> GetComboAsync()
    {
        return Ok(await _reelsUnitOfWork.GetComboAsync());
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync()
    {
        var response = await _reelsUnitOfWork.GetAsync();
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _reelsUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
    {
        var response = await _reelsUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecordsPaginated")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _reelsUnitOfWork.GetTotalRecordsAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Seller")]
    [HttpPost]
    public override async Task<IActionResult> PostAsync(Reel model)
    {
        var response = await base.PostAsync(model);
        if (response is OkObjectResult)
        {
            return response;
        }
        return BadRequest();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Seller")]
    [HttpPut]
    public override async Task<IActionResult> PutAsync(Reel model)
    {
        var response = await base.PutAsync(model);
        if (response is OkObjectResult)
        {
            return response;
        }
        return BadRequest();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Seller")]
    [HttpDelete("{id}")]
    public override async Task<IActionResult> DeleteAsync(int id)
    {
        var response = await base.DeleteAsync(id);
        if (response is NoContentResult)
        {
            return response;
        }
        return BadRequest();
    }
}