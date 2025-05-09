using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Interfaces;

public interface ICommentsUnitOfWork
{
    Task<ActionResponse<Comments>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Comments>>> GetAsync();

    Task<ActionResponse<IEnumerable<Comments>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<IEnumerable<Comments>>> GetCommentsByProductAsync(PaginationDTO pagination);

    Task<ActionResponse<Comments>> AddAsync(CommetDTO comment);
}