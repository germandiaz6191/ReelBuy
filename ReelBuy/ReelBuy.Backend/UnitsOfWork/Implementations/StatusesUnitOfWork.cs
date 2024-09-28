using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class StatusesUnitOfWork : GenericUnitOfWork<Status>, IStatusesUnitOfWork
{
    private readonly IStatusesRepository _statusesRepository;

    public StatusesUnitOfWork(IGenericRepository<Status> repository, IStatusesRepository statusesRepository) : base(repository)
    {
        _statusesRepository = statusesRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Status>>> GetAsync() => await _statusesRepository.GetAsync();

    public override async Task<ActionResponse<Status>> GetAsync(int id) => await _statusesRepository.GetAsync(id);

    public async Task<IEnumerable<Status>> GetComboAsync() => await _statusesRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Status>>> GetAsync(PaginationDTO pagination) => await _statusesRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _statusesRepository.GetTotalRecordsAsync(pagination);
}