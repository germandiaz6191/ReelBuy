using System.ComponentModel.DataAnnotations;

namespace ReelBuy.Shared.Entities;

public class Product
{
    public int Id { get; set; }

    [MaxLength(200)]
    [Required]
    public String Name { get; set; } = null!;
}