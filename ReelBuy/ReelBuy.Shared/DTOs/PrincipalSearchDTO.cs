using ReelBuy.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReelBuy.Shared.DTOs;

public class PrincipalSearchDTO
{
    public string keyword { get; set; } = null!;
}