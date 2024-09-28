using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Interfaces;

public interface IReputationsRepository
{
    Task<ActionResponse<Reputation>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Reputation>>> GetAsync();

    Task<IEnumerable<Reputation>> GetComboAsync();

    Task<ActionResponse<IEnumerable<Reputation>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}