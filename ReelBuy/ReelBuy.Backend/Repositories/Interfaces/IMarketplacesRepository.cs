using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Interfaces;

public interface IMarketplacesRepository
{
    Task<ActionResponse<Marketplace>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Marketplace>>> GetAsync();

    Task<IEnumerable<Marketplace>> GetComboAsync();

    Task<ActionResponse<IEnumerable<Marketplace>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}