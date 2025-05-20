using Microsoft.AspNetCore.Mvc;
using ReelBuy.Backend.Services;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.DTOs;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ReelBuy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class VideoGenerationController : ControllerBase
{
    private readonly IVideoGenerationService _videoGenerationService;
    private readonly IUsersUnitOfWork _usersUnitOfWork;

    public VideoGenerationController(IVideoGenerationService videoGenerationService, IUsersUnitOfWork usersUnitOfWork)
    {
        _videoGenerationService = videoGenerationService;
        _usersUnitOfWork = usersUnitOfWork;
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

            var generatedVideo = await _videoGenerationService.GenerateVideoAsync(
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

            var updatedVideos = await _videoGenerationService.UpdateVideosStatusAsync(request.VideoId);
            return Ok(updatedVideos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error checking video status: {ex.Message}");
        }
    }
} 