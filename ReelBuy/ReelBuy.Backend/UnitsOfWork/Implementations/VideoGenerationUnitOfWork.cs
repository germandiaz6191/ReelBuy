using ReelBuy.Backend.Services;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class VideoGenerationUnitOfWork : IVideoGenerationUnitOfWork
{
    private readonly IVideoGenerationService _videoGenerationService;

    public VideoGenerationUnitOfWork(IVideoGenerationService videoGenerationService)
    {
        _videoGenerationService = videoGenerationService;
    }

    public async Task<GeneratedVideo> GenerateVideoAsync(string userId, string prompt, string voice, string theme, string language) => await _videoGenerationService.GenerateVideoAsync(userId, prompt, voice, theme, language);

    public async Task<GeneratedVideo> UpdateVideosStatusAsync(long videoId) => await _videoGenerationService.UpdateVideosStatusAsync(videoId);

    public async Task<ActionResponse<IEnumerable<GeneratedVideo>>> GetAsync(PaginationDTO pagination) => await _videoGenerationService.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _videoGenerationService.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<GeneratedVideo>> GetVideoUrlAsync(long videoId) => await _videoGenerationService.GetVideoUrlAsync(videoId);
}