using ReelBuy.Shared.Entities;
using System.Diagnostics.CodeAnalysis;

namespace ReelBuy.Shared.DTOs;

[ExcludeFromCodeCoverage]
public class DepartmentDTO
{
    public Department Department { get; set; } = null!;

    public int CountryId { get; set; }
}