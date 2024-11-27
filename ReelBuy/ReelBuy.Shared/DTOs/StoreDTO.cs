using ReelBuy.Shared.Entities;

namespace ReelBuy.Shared.DTOs;

public class StoreDTO
{
    public int Id { get; set; }
    public Store Store { get; set; } = null!;
    public int CityId { get; set; }
}