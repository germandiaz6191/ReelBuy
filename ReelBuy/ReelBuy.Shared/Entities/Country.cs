using System.ComponentModel.DataAnnotations;

namespace ReelBuy.Shared.Entities;

public class Country
{
    public int Id { get; set; }

    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = null!;
}