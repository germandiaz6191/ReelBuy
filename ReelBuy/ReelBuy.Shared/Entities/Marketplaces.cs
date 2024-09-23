using System.ComponentModel.DataAnnotations;

namespace ReelBuy.Shared.Entities;

public class Marketplace
{
    public int Id { get; set; }

    [MaxLength(200)]
    [Required]
    public String Name { get; set; } = null!;

    [MaxLength(60)]
    [Required]
    public String Domain { get; set; } = null!;
}