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

    [Display(Name = "Description", ResourceType = typeof(Literals))]
    [MaxLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Description { get; set; } = null!;

    [Display(Name = "Price", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0")]
    public decimal Price { get; set; }

    public ICollection<Reel> Reels { get; set; } = new List<Reel>();
    public int StatusId { get; set; }
    public Status? Status { get; set; }
    public int MarketplaceId { get; set; }
    public Marketplace? Marketplace { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<Favorite>? Favorites { get; set; }
    public ICollection<Comments>? Comments { get; set; }
    public int StoreId { get; set; }
    public Store? Store { get; set; }
    public int LikesGroup { get; set; }

    public virtual ICollection<User>? LikedBy { get; set; }

    [Display(Name = "MotiveReject", ResourceType = typeof(Literals))]
    [MaxLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string? MotiveReject { get; set; }

    public static implicit operator Product(int v)
    {
        throw new NotImplementedException();
    }
}