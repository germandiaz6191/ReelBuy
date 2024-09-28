using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Implementations;

public class ReputationsRepository : GenericRepository<Reputation>, IReputationsRepository
{
    private readonly DataContext _context;

    public ReputationsRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Reputation>>> GetAsync()
    {
        var reputations = await _context.Reputations
            .OrderBy(c => c.Score)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Reputation>>
        {
            WasSuccess = true,
            Result = reputations
        };
    }

    public override async Task<ActionResponse<Reputation>> GetAsync(int id)
    {
        var reputations = await _context.Reputations
             .FirstOrDefaultAsync(c => c.Id == id);

        if (reputations == null)
        {
            return new ActionResponse<Reputation>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Reputation>
        {
            WasSuccess = true,
            Result = reputations
        };
    }

    public async Task<IEnumerable<Reputation>> GetComboAsync()
    {
        return await _context.Reputations
             .OrderBy(c => c.Score)
         .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Reputation>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Reputations
                    .AsQueryable();
        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Score.Equals(pagination.Filter));
        }

        return new ActionResponse<IEnumerable<Reputation>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Score)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Reputations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Score.Equals(pagination.Filter));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }
}