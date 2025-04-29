using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Interfaces;

public interface ILikesRepository
{
    Task<ActionResponse<int>> AddAsync(LikeDTO dto);

    Task<ActionResponse<int>> DeleteAsync(string userId, int productId);
}