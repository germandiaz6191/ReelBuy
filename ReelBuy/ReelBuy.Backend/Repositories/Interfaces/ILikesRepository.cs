using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Interfaces;

public interface ILikesRepository
{
    Task<ActionResponse<bool>> GetAsync(string userId, int productId);

    Task<ActionResponse<int>> AddAsync(LikeDTO dto);

    Task<ActionResponse<int>> DeleteAsync(string userId, int productId);
}