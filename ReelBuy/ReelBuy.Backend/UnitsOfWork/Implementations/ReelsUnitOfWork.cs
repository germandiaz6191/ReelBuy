using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class ReelsUnitOfWork : GenericUnitOfWork<Reel>, IReelsUnitOfWork
{
    private readonly IReelsRepository _reelsRepository;

    public ReelsUnitOfWork(IGenericRepository<Reel> repository, IReelsRepository reelsRepository) : base(repository)
    {
        _reelsRepository = reelsRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Reel>>> GetAsync() => await _reelsRepository.GetAsync();

    public override async Task<ActionResponse<Reel>> GetAsync(int id) => await _reelsRepository.GetAsync(id);

    public async Task<IEnumerable<Reel>> GetComboAsync() => await _reelsRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Reel>>> GetAsync(PaginationDTO pagination) => await _reelsRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _reelsRepository.GetTotalRecordsAsync(pagination);
}