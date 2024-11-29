using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Interfaces;

public interface IProductsUnitOfWork
{
    Task<ActionResponse<Product>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Product>>> GetAsync();

    Task<IEnumerable<Product>> GetComboAsync();

    Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<IEnumerable<Product>>> GetProductsByLikeAsync(PrincipalSearchDTO principalSearch);
}