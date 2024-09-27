using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Interfaces
{
    public interface IProfilesUnitOfWork
    {
        Task<ActionResponse<Profiles>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Profiles>>> GetAsync();

        Task<IEnumerable<Profiles>> GetComboAsync();

        Task<ActionResponse<IEnumerable<Profiles>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    }
}
