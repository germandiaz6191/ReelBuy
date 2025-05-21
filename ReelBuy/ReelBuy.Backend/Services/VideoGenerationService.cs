using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ReelBuy.Backend.Data;
using ReelBuy.Shared.Entities;
using System.Text.Json.Serialization;
using SixLabors.ImageSharp;
using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Helpers;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Responses;
using ReelBuy.Backend.Repositories.Implementations;
using Microsoft.Identity.Client;
using Org.BouncyCastle.Asn1.Ocsp;

namespace ReelBuy.Backend.Services;

public interface IVideoGenerationService
{
    Task<GeneratedVideo> GenerateVideoAsync(string userId, string prompt, string voice = "Charlie", string theme = "Hormozi_1", string language = "Spanish");

    Task<GeneratedVideo> UpdateVideosStatusAsync(long videoId);

    Task<ActionResponse<IEnumerable<GeneratedVideo>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<GeneratedVideo>> GetVideoUrlAsync(long videoId);
}

public class VideoGenerationService : GenericRepository<GeneratedVideo>, IVideoGenerationService
{
    private readonly IConfiguration _configuration;
    private readonly DataContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<VideoGenerationService> _logger;

    public VideoGenerationService(
        IConfiguration configuration,
        DataContext context,
        IHttpClientFactory httpClientFactory,
        ILogger<VideoGenerationService> logger) : base(context)
    {
        _configuration = configuration;
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<GeneratedVideo> GenerateVideoAsync(string userId, string prompt, string voice = "Charlie", string theme = "Hormozi_1", string language = "Spanish")
    {
        var requestBody = new
        {
            topic = "Custom",
            prompt = prompt,
            voice = voice,
            theme = theme,
            style = "None",
            language = language,
            duration = "30-60",
            aspect_ratio = "9:16"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        //var response = await _httpClient.PostAsync(_apiUrl, content);
        //response.EnsureSuccessStatusCode();
        //var responseContent = await response.Content.ReadAsStringAsync();
        //var responseData = JsonSerializer.Deserialize<VideoResponse>(responseContent);
        var responseData = new VideoResponse
        {
            VideoId = 396150314229
        };

        if (responseData?.VideoId == null)
        {
            throw new Exception("Failed to generate video: Invalid response from API");
        }

        var generatedVideo = new GeneratedVideo
        {
            UserId = userId,
            VideoId = responseData.VideoId,
            Prompt = prompt,
            Voice = voice,
            Theme = theme,
            Language = language,
            StatusDetail = "pending"
        };

        _context.GeneratedVideos.Add(generatedVideo);
        await _context.SaveChangesAsync();

        return generatedVideo;
    }

    public async Task<GeneratedVideo> UpdateVideosStatusAsync(long videoId)
    {
        var client = _httpClientFactory.CreateClient("VadooAPI");
        var apiKey = _configuration["VadooAPI:ApiKey"];
        client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

        var video = await _context.GeneratedVideos.FirstOrDefaultAsync(v => v.VideoId == videoId);

        try
        {
            var response = await client.GetAsync($"https://viralapi.vadoo.tv/api/get_video_url?id={video.VideoId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error checking video status: {ResponseContent}", responseContent);
                video.StatusDetail = $"Error checking status: {responseContent}";
            }
            var statusResponse = JsonSerializer.Deserialize<VadooVideoStatusResponse>(responseContent);
            video.StatusDetail = statusResponse!.Status;
            video.VideoUrl = statusResponse.Url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating video status for video {VideoId}", video.Id);
            video.StatusDetail = "error";
            video.StatusDetail = $"Error: {ex.Message}";
        }

        await _context.SaveChangesAsync();
        return video;
    }

    public async Task<ActionResponse<GeneratedVideo>> GetVideoUrlAsync(long videoId)
    {
        var client = _httpClientFactory.CreateClient("VadooAPI");
        var apiKey = _configuration["VadooAPI:ApiKey"];
        client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);

        var video = await _context.GeneratedVideos.FirstOrDefaultAsync(v => v.VideoId == videoId);

        try
        {
            var response = await client.GetAsync($"https://viralapi.vadoo.tv/api/get_video_url?id={video.VideoId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error checking video status: {ResponseContent}", responseContent);
                video.StatusDetail = $"Error checking status: {responseContent}";
            }
            var statusResponse = JsonSerializer.Deserialize<VadooVideoStatusResponse>(responseContent);
            video.StatusDetail = statusResponse!.Status;
            video.VideoUrl = statusResponse.Url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating video status for video {VideoId}", video.Id);
            video.StatusDetail = "error";
            video.StatusDetail = $"Error: {ex.Message}";
        }

        var generatedVideo = new GeneratedVideo
        {
            VideoId = video.VideoId,
            VideoUrl = video.VideoUrl,
            StatusDetail = video.StatusDetail
        };

        return new ActionResponse<GeneratedVideo>
        {
            WasSuccess = true,
            Result = generatedVideo
        };
    }

    public override async Task<ActionResponse<IEnumerable<GeneratedVideo>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.GeneratedVideos
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.UserId.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<GeneratedVideo>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.CreatedAt)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.GeneratedVideos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.UserId.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }
}

public class VideoResponse
{
    [JsonPropertyName("vid")]
    public long VideoId { get; set; }
}