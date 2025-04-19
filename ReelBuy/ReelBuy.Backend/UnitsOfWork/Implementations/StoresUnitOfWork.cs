using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class StoresUnitOfWork : GenericUnitOfWork<Store>, IStoresUnitOfWork
{
    private readonly IStoresRepository _storesRepository;

    public StoresUnitOfWork(IGenericRepository<Store> repository, IStoresRepository storesRepository) : base(repository)
    {
        _storesRepository = storesRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Store>>> GetAsync() => await _storesRepository.GetAsync();

    public override async Task<ActionResponse<Store>> GetAsync(int id) => await _storesRepository.GetAsync(id);

    public async Task<IEnumerable<Store>> GetComboAsync() => await _storesRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Store>>> GetAsync(PaginationDTO pagination) => await _storesRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _storesRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<Store>> AddAsync(StoreDTO storeDTO) => await _storesRepository.AddAsync(storeDTO);

    public async Task<ActionResponse<Store>> UpdateAsync(StoreDTO storeDTO) => await _storesRepository.UpdateAsync(storeDTO);

    public async Task<ActionResponse<IEnumerable<Store>>> GetStoresByUserAsync(string userId) => await _storesRepository.GetStoresByUserAsync(userId);
}