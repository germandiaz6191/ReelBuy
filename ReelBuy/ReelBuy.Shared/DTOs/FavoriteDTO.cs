using ReelBuy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace ReelBuy.Shared.DTOs
{
    public class FavoriteDTO
    {
        public int Id { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Literals))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
        public string? Name { get; set; }

        [Display(Name = "Product", ResourceType = typeof(Literals))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
        public int ProductId { get; set; }

        [Display(Name = "User", ResourceType = typeof(Literals))]
        [MaxLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
        public string UserId { get; set; } = null!;
    }
}