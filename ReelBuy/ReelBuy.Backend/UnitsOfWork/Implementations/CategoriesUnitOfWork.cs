using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class CategoriesUnitOfWork : GenericUnitOfWork<Category>, ICategoriesUnitOfWork
{
    private readonly ICategoriesRepository _categoriesRepository;

    public CategoriesUnitOfWork(IGenericRepository<Category> repository, ICategoriesRepository categoriesRepository) : base(repository)
    {
        _categoriesRepository = categoriesRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Category>>> GetAsync() => await _categoriesRepository.GetAsync();

    public override async Task<ActionResponse<Category>> GetAsync(int id) => await _categoriesRepository.GetAsync(id);

    public async Task<IEnumerable<Category>> GetComboAsync() => await _categoriesRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Category>>> GetAsync(PaginationDTO pagination) => await _categoriesRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _categoriesRepository.GetTotalRecordsAsync(pagination);
}