using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Interfaces;

public interface IFavoritesRepository
{
    Task<ActionResponse<Favorite>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Favorite>>> GetAsync();

    Task<IEnumerable<Favorite>> GetComboAsync();

    Task<ActionResponse<IEnumerable<Favorite>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<Favorite>> AddAsync(FavoriteDTO cityDTO);
}