namespace ReelBuy.Shared.DTOs;

public class VideoGenerationRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string Voice { get; set; } = "Charlie";
    public string Theme { get; set; } = "Hormozi_1";
    public string Language { get; set; } = "Spanish";
} 