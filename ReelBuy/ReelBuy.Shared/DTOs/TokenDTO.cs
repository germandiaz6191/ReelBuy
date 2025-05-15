using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace ReelBuy.Shared.DTOs;

[ExcludeFromCodeCoverage]
public class TokenDTO
{
    public string Token { get; set; } = null!;

    public DateTime Expiration { get; set; }
}