using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Interfaces;

public interface ICategoriesUnitOfWork
{
    Task<ActionResponse<Category>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Category>>> GetAsync();

    Task<IEnumerable<Category>> GetComboAsync();

    Task<ActionResponse<IEnumerable<Category>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}