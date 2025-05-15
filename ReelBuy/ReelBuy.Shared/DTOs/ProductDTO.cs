using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ReelBuy.Shared.DTOs;

[ExcludeFromCodeCoverage]
public class ProductDTO
{
    public int Id { get; set; }

    [Display(Name = "Name", ResourceType = typeof(Literals))]
    [MaxLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Display(Name = "Description", ResourceType = typeof(Literals))]
    [MaxLength(4000, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Description { get; set; } = null!;

    [Display(Name = "Price", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
    public decimal Price { get; set; }

    public ICollection<Reel> Reels { get; set; } = new List<Reel>();
    public int StatusId { get; set; }
    public int MarketplaceId { get; set; }
    public int CategoryId { get; set; }
    public int StoreId { get; set; }
}