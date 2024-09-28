using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class ReputationsUnitOfWork : GenericUnitOfWork<Reputation>, IReputationsUnitOfWork
{
    private readonly IReputationsRepository _reputationsRepository;

    public ReputationsUnitOfWork(IGenericRepository<Reputation> repository, IReputationsRepository reputationsRepository) : base(repository)
    {
        _reputationsRepository = reputationsRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Reputation>>> GetAsync() => await _reputationsRepository.GetAsync();

    public override async Task<ActionResponse<Reputation>> GetAsync(int id) => await _reputationsRepository.GetAsync(id);

    public async Task<IEnumerable<Reputation>> GetComboAsync() => await _reputationsRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Reputation>>> GetAsync(PaginationDTO pagination) => await _reputationsRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _reputationsRepository.GetTotalRecordsAsync(pagination);
}