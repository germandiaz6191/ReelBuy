using ReelBuy.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReelBuy.Shared.Entities;

public class Reel
{
    public int Id { get; set; }

    [Display(Name = "Name", ResourceType = typeof(Literals))]
    [MaxLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public string ReelUri { get; set; } = null!;

    [NotMapped]
    public string? Base64 { get; set; }
}