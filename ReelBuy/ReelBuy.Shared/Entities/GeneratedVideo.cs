using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReelBuy.Shared.Entities;

public class GeneratedVideo
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public long VideoId { get; set; }

    [Required]
    public string Prompt { get; set; } = null!;

    [Required]
    public string Voice { get; set; } = null!;

    [Required]
    public string Theme { get; set; } = null!;

    [Required]
    public string Language { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    public string? VideoUrl { get; set; }
    
    public string? StatusDetail { get; set; }
} 