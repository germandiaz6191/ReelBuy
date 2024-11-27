using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Interfaces;

public interface IStoresRepository
{
    Task<ActionResponse<Store>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Store>>> GetAsync();

    Task<IEnumerable<Store>> GetComboAsync();

    Task<ActionResponse<IEnumerable<Store>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<Store>> AddAsync(StoreDTO storeDTO);

    Task<ActionResponse<Store>> UpdateAsync(StoreDTO storeDTO);
}