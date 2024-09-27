using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class CategoriesUnitOfWork : GenericUnitOfWork<Categories>, ICategoriesUnitOfWork
{
    private readonly ICategoriesRepository _categoriesRepository;

    public CategoriesUnitOfWork(IGenericRepository<Categories> repository, ICategoriesRepository categoriesRepository) : base(repository)
    {
        _categoriesRepository = categoriesRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Categories>>> GetAsync() => await _categoriesRepository.GetAsync();

    public override async Task<ActionResponse<Categories>> GetAsync(int id) => await _categoriesRepository.GetAsync(id);

    public async Task<IEnumerable<Categories>> GetComboAsync() => await _categoriesRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Categories>>> GetAsync(PaginationDTO pagination) => await _categoriesRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _categoriesRepository.GetTotalRecordsAsync(pagination);
}
