using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class ProductsUnitOfWork : GenericUnitOfWork<Product>, IProductsUnitOfWork
{
    private readonly IProductsRepository _productsRepository;

    public ProductsUnitOfWork(IGenericRepository<Product> repository, IProductsRepository productsRepository) : base(repository)
    {
        _productsRepository = productsRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Product>>> GetAsync() => await _productsRepository.GetAsync();

    public override async Task<ActionResponse<Product>> GetAsync(int id) => await _productsRepository.GetAsync(id);

    public async Task<IEnumerable<Product>> GetComboAsync() => await _productsRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination) => await _productsRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _productsRepository.GetTotalRecordsAsync(pagination);
}