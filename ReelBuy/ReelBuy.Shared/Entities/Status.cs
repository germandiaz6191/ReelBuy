using System.ComponentModel.DataAnnotations;

namespace ReelBuy.Shared.Entities;

public class Status
{
    public int Id { get; set; }

    [MaxLength(50)]
    [Required]
    public String Name { get; set; } = null!;
}