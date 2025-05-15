using ReelBuy.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace ReelBuy.Shared.DTOs;

[ExcludeFromCodeCoverage]
public class PrincipalSearchDTO
{
    public string keyword { get; set; } = null!;
}