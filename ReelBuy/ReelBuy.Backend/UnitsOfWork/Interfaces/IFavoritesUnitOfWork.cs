using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public interface IFavoritesUnitOfWork
{
    Task<ActionResponse<Favorite>> GetAsync(int id);

    Task<ActionResponse<Favorite>> GetAsync(string userId, int productId);

    Task<ActionResponse<IEnumerable<Favorite>>> GetAsync();

    Task<IEnumerable<Favorite>> GetComboAsync();

    Task<ActionResponse<IEnumerable<Favorite>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<Favorite>> AddAsync(FavoriteDTO favoriteDTO);
}