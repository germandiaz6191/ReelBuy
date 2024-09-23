using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Interfaces
{
    public interface IStatusesRepository
    {
        Task<ActionResponse<Status>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Status>>> GetAsync();

        Task<IEnumerable<Status>> GetComboAsync();

        Task<ActionResponse<IEnumerable<Status>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    }
}