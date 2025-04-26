using ReelBuy.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReelBuy.Shared.Entities;

public class Comments
{
    public int Id { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Literals))]
    [MaxLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string? Description { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
}