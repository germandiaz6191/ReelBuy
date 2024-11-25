using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Interfaces;

public interface ICitiesRepository
{
    Task<ActionResponse<City>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<City>>> GetAsync();

    Task<IEnumerable<City>> GetComboAsync();

    Task<ActionResponse<IEnumerable<City>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}