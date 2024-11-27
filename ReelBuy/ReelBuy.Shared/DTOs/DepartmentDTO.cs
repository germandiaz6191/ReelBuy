using ReelBuy.Shared.Entities;

namespace ReelBuy.Shared.DTOs;

public class DepartmentDTO
{
    public Department Department { get; set; } = null!;

    public int CountryId { get; set; }
}