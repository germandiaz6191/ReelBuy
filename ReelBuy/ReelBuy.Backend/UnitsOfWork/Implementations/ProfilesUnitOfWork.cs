using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class ProfilesUnitOfWork : GenericUnitOfWork<Profiles>, IProfilesUnitOfWork
{
    private readonly IProfilesRepository _profilesRepository;

    public ProfilesUnitOfWork(IGenericRepository<Profiles> repository, IProfilesRepository profilesRepository) : base(repository)
    {
        _profilesRepository = profilesRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Profiles>>> GetAsync() => await _profilesRepository.GetAsync();

    public override async Task<ActionResponse<Profiles>> GetAsync(int id) => await _profilesRepository.GetAsync(id);

    public async Task<IEnumerable<Profiles>> GetComboAsync() => await _profilesRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Profiles>>> GetAsync(PaginationDTO pagination) => await _profilesRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _profilesRepository.GetTotalRecordsAsync(pagination);
}
