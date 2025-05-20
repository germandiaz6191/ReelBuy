namespace ReelBuy.Shared.DTOs;
using System.Text.Json.Serialization;

public class VadooVideoStatusResponse
{
     [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
     [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
} 