using System.Diagnostics.CodeAnalysis;

namespace ReelBuy.Shared.DTOs;

[ExcludeFromCodeCoverage]
public class PaginationDTO
{
    public int Id { get; set; }

    public int Page { get; set; } = 1;

    public int RecordsNumber { get; set; } = 10;

    public string? Filter { get; set; }

    public int? FilterStatus { get; set; }
    
    public string? StoreIds { get; set; }
    
    public string? UserId { get; set; }
}