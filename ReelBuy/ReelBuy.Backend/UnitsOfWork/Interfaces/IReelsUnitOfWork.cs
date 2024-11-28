using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Interfaces;

public interface IReelsUnitOfWork
{
    Task<ActionResponse<Reel>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Reel>>> GetAsync();

    Task<IEnumerable<Reel>> GetComboAsync();

    Task<ActionResponse<IEnumerable<Reel>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}