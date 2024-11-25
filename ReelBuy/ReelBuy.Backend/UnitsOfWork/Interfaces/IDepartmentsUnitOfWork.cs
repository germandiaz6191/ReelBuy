using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public interface IDepartmentsUnitOfWork
{
    Task<ActionResponse<Department>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Department>>> GetAsync();

    Task<IEnumerable<Department>> GetComboAsync();

    Task<ActionResponse<IEnumerable<Department>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}