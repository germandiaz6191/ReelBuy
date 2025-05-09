﻿using ReelBuy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace ReelBuy.Shared.Entities;

public class Comments
{
    public int Id { get; set; }

    [Display(Name = "Description", ResourceType = typeof(Literals))]
    [MaxLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Description { get; set; } = null!;

    [Display(Name = "RegistrationDate", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public DateTime RegistrationDate { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
}