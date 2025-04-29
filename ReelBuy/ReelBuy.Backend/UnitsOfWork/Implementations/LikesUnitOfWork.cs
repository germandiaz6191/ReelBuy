using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class LikesUnitOfWork : ILikesUnitOfWork
{
    private readonly ILikesRepository _likesRepository;

    public LikesUnitOfWork(ILikesRepository likesRepository)
    {
        _likesRepository = likesRepository;
    }

    public virtual async Task<ActionResponse<int>> AddAsync(LikeDTO entity) => await _likesRepository.AddAsync(entity);

    public virtual async Task<ActionResponse<int>> DeleteAsync(string userId, int productId) => await _likesRepository.DeleteAsync(userId, productId);
}