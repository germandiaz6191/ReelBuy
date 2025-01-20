using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class FavoritesUnitOfWork : GenericUnitOfWork<Favorite>, IFavoritesUnitOfWork
{
    private readonly IFavoritesRepository _favoritesRepository;

    public FavoritesUnitOfWork(IGenericRepository<Favorite> repository, IFavoritesRepository favoritesRepository) : base(repository)
    {
        _favoritesRepository = favoritesRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Favorite>>> GetAsync() => await _favoritesRepository.GetAsync();

    public override async Task<ActionResponse<Favorite>> GetAsync(int id) => await _favoritesRepository.GetAsync(id);

    public async Task<IEnumerable<Favorite>> GetComboAsync() => await _favoritesRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Favorite>>> GetAsync(PaginationDTO pagination) => await _favoritesRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _favoritesRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<Favorite>> AddAsync(FavoriteDTO favoriteDTO) => await _favoritesRepository.AddAsync(favoriteDTO);
}