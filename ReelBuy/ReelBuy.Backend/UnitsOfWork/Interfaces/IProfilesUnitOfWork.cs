using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Interfaces;

public interface IProfilesUnitOfWork
{
    Task<ActionResponse<Profile>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Profile>>> GetAsync();

    Task<IEnumerable<Profile>> GetComboAsync();

    Task<ActionResponse<IEnumerable<Profile>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}