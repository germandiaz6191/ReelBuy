using ReelBuy.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ReelBuy.Shared.DTOs;

[ExcludeFromCodeCoverage]
public class EmailDTO
{
    [Display(Name = "Email", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [EmailAddress(ErrorMessageResourceName = "ValidEmail", ErrorMessageResourceType = typeof(Literals))]
    public string Email { get; set; } = null!;

    public string Language { get; set; } = null!;
}