using Microsoft.AspNetCore.Mvc;
using ReelBuy.Backend.Services;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.DTOs;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Azure;

namespace ReelBuy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class VideoGenerationController : GenericController<GeneratedVideo>
{
    private readonly IVideoGenerationUnitOfWork _videoGenerationUnitOfWork;
    private readonly IUsersUnitOfWork _usersUnitOfWork;

    public VideoGenerationController(IGenericUnitOfWork<GeneratedVideo> unit, IVideoGenerationUnitOfWork videoGenerationUnitOfWork, IUsersUnitOfWork usersUnitOfWork) : base(unit)
    {
        _videoGenerationUnitOfWork = videoGenerationUnitOfWork;
        _usersUnitOfWork = usersUnitOfWork;
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
    {
        bool isAdmin = User.IsInRole("Admin");
        if (!isAdmin)
        {
            var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            pagination.Filter = currentUser.Id;
        }

        var response = await _videoGenerationUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecordsPaginated")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        bool isAdmin = User.IsInRole("Admin");
        if (!isAdmin)
        {
            var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            pagination.Filter = currentUser.Id;
        }

        var action = await _videoGenerationUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("video/{id}")]
    public async Task<ActionResult<List<GeneratedVideo>>> GetVideoUrlAsync(long id)
    {
        var response = await _videoGenerationUnitOfWork.GetVideoUrlAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpPost("generate")]
    public async Task<ActionResult<GeneratedVideo>> GenerateVideo([FromBody] VideoGenerationRequest request)
    {
        try
        {
            var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            string userId = currentUser.Id;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var generatedVideo = await _videoGenerationUnitOfWork.GenerateVideoAsync(
                userId,
                request.Prompt,
                request.Voice,
                request.Theme,
                request.Language
            );

            return Ok(generatedVideo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error generating video: {ex.Message}");
        }
    }

    [HttpPost("check-status")]
    public async Task<ActionResult<List<GeneratedVideo>>> CheckVideosStatus([FromBody] VideoStatusUpdateRequest request)
    {
        try
        {
            var currentUser = await _usersUnitOfWork.GetUserAsync(User.Identity!.Name!);
            string userId = currentUser.Id;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var updatedVideos = await _videoGenerationUnitOfWork.UpdateVideosStatusAsync(request.VideoId);
            return Ok(updatedVideos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error checking video status: {ex.Message}");
        }
    }
}