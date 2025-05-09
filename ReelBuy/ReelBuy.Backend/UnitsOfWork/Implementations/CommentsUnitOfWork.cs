using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class CommentsUnitOfWork : GenericUnitOfWork<Comments>, ICommentsUnitOfWork
{
    private readonly ICommentsRepository _commentsRepository;

    public CommentsUnitOfWork(IGenericRepository<Comments> repository, ICommentsRepository commentsRepository) : base(repository)
    {
        _commentsRepository = commentsRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Comments>>> GetAsync() => await _commentsRepository.GetAsync();

    public override async Task<ActionResponse<Comments>> GetAsync(int id) => await _commentsRepository.GetAsync(id);

    public override async Task<ActionResponse<IEnumerable<Comments>>> GetAsync(PaginationDTO pagination) => await _commentsRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _commentsRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<IEnumerable<Comments>>> GetCommentsByProductAsync(PaginationDTO pagination) => await _commentsRepository.GetCommentsByProductAsync(pagination);

    public async Task<ActionResponse<Comments>> AddAsync(CommetDTO comment) => await _commentsRepository.AddAsync(comment);
}