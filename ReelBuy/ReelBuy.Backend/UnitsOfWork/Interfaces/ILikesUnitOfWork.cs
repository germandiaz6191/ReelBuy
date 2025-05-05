using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Interfaces;

public interface ILikesUnitOfWork
{
    Task<ActionResponse<bool>> GetAsync(string userId, int productId);

    Task<ActionResponse<int>> AddAsync(LikeDTO entity);

    Task<ActionResponse<int>> DeleteAsync(string userId, int productId);
}