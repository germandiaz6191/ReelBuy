using ReelBuy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace ReelBuy.Shared.Entities;

public class Status
{
    public int Id { get; set; }

    [Display(Name = "Name", ResourceType = typeof(Literals))]
    [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;
}
