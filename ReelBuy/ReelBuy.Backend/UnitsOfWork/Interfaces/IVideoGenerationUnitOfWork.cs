using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Interfaces;

public interface IVideoGenerationUnitOfWork
{
    Task<GeneratedVideo> GenerateVideoAsync(string userId, string prompt, string voice, string theme, string language);
    Task<GeneratedVideo> UpdateVideosStatusAsync(long videoId);
    Task<ActionResponse<IEnumerable<GeneratedVideo>>> GetAsync(PaginationDTO pagination);
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<string>> GetVideoUrlAsync(long videoId);
}
