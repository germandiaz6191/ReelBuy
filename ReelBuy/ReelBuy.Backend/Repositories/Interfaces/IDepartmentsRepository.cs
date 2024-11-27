using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Interfaces;

public interface IDepartmentsRepository
{
    Task<ActionResponse<Department>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Department>>> GetAsync();

    Task<IEnumerable<Department>> GetComboAsync();

    Task<ActionResponse<IEnumerable<Department>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<Department>> AddAsync(DepartmentDTO departmentDTO);

    Task<ActionResponse<Department>> UpdateAsync(DepartmentDTO departmentDTO);
}