using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class ProfilesUnitOfWork : GenericUnitOfWork<Profile>, IProfilesUnitOfWork
{
    private readonly IProfilesRepository _profilesRepository;

    public ProfilesUnitOfWork(IGenericRepository<Profile> repository, IProfilesRepository profilesRepository) : base(repository)
    {
        _profilesRepository = profilesRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Profile>>> GetAsync() => await _profilesRepository.GetAsync();

    public override async Task<ActionResponse<Profile>> GetAsync(int id) => await _profilesRepository.GetAsync(id);

    public async Task<IEnumerable<Profile>> GetComboAsync() => await _profilesRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Profile>>> GetAsync(PaginationDTO pagination) => await _profilesRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _profilesRepository.GetTotalRecordsAsync(pagination);
}