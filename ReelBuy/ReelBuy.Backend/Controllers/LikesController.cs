using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;

namespace ReelBuy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LikesController : ControllerBase
{
    private readonly ILikesUnitOfWork _likesUnitOfWork;

    public LikesController(ILikesUnitOfWork likesUnitOfWork)
    {
        _likesUnitOfWork = likesUnitOfWork;
    }

    [HttpGet("{userId}/{productId}")]
    public async Task<IActionResult> GetLikeAsync(string userId, int productId)
    {
        var action = await _likesUnitOfWork.GetAsync(userId, productId);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return NotFound(action.Message);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("PostLikeAsync")]
    public async Task<IActionResult> PostAsync(LikeDTO model)
    {
        var action = await _likesUnitOfWork.AddAsync(model);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("DeleteLikeAsync/{userId}/{productId}")]
    public async Task<IActionResult> DeleteLikeAsync(string userId, int productId)
    {
        var action = await _likesUnitOfWork.DeleteAsync(userId, productId);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }
}