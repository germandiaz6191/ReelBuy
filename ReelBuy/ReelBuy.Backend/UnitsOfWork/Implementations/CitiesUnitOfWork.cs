using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class CitiesUnitOfWork : GenericUnitOfWork<City>, ICitiesUnitOfWork
{
    private readonly ICitiesRepository _citiesRepository;

    public CitiesUnitOfWork(IGenericRepository<City> repository, ICitiesRepository citiesRepository) : base(repository)
    {
        _citiesRepository = citiesRepository;
    }

    public override async Task<ActionResponse<IEnumerable<City>>> GetAsync() => await _citiesRepository.GetAsync();

    public override async Task<ActionResponse<City>> GetAsync(int id) => await _citiesRepository.GetAsync(id);

    public async Task<IEnumerable<City>> GetComboAsync() => await _citiesRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<City>>> GetAsync(PaginationDTO pagination) => await _citiesRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _citiesRepository.GetTotalRecordsAsync(pagination);
}