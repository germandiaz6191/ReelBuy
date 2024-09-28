using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class MarketplacesUnitOfWork : GenericUnitOfWork<Marketplace>, IMarketplacesUnitOfWork
{
    private readonly IMarketplacesRepository _marketplacesRepository;

    public MarketplacesUnitOfWork(IGenericRepository<Marketplace> repository, IMarketplacesRepository marketplacesRepository) : base(repository)
    {
        _marketplacesRepository = marketplacesRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Marketplace>>> GetAsync() => await _marketplacesRepository.GetAsync();

    public override async Task<ActionResponse<Marketplace>> GetAsync(int id) => await _marketplacesRepository.GetAsync(id);

    public async Task<IEnumerable<Marketplace>> GetComboAsync() => await _marketplacesRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Marketplace>>> GetAsync(PaginationDTO pagination) => await _marketplacesRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _marketplacesRepository.GetTotalRecordsAsync(pagination);
}