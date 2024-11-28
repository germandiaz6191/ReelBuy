
using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Implementations;

public class ReelsRepository : GenericRepository<Reel>, IReelsRepository
{
    private readonly DataContext _context;

    public ReelsRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Reel>>> GetAsync()
    {
        var reels = await _context.Reels
            .OrderBy(c => c.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Reel>>
        {
            WasSuccess = true,
            Result = reels
        };
    }

    public override async Task<ActionResponse<Reel>> GetAsync(int id)
    {
        var reels = await _context.Reels
             .FirstOrDefaultAsync(c => c.Id == id);

        if (reels == null)
        {
            return new ActionResponse<Reel>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Reel>
        {
            WasSuccess = true,
            Result = reels
        };
    }

    public async Task<IEnumerable<Reel>> GetComboAsync()
    {
        return await _context.Reels
            .OrderBy(c => c.Name)
        .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Reel>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Reels
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Reel>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Reels.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }
}
