using ReelBuy.Shared.Entities;

namespace ReelBuy.Shared.DTOs;

public class CityDTO
{
    public City City { get; set; } = null!;
    public int DepartmentId { get; set; }
}