using ReelBuy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace ReelBuy.Shared.Entities;

public class Product
{
    public int Id { get; set; }

    [Display(Name = "Name", ResourceType = typeof(Literals))]
    [MaxLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    public ICollection<Reel> Reels { get; set; } = new List<Reel>();
    public int StatusId { get; set; }
    public Status? Status { get; set; }
    public int MarketplaceId { get; set; }
    public Marketplace? Marketplace { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public int StoreId { get; set; }
    public Store? Store { get; set; }
}