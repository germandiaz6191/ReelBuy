using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Implementations;

public class StatusesRepository : GenericRepository<Status>, IStatusesRepository
{
    private readonly DataContext _context;

    public StatusesRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Status>>> GetAsync()
    {
        var statuses = await _context.Statuses
            .OrderBy(c => c.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Status>>
        {
            WasSuccess = true,
            Result = statuses
        };
    }

    public override async Task<ActionResponse<Status>> GetAsync(int id)
    {
        var status = await _context.Statuses
             .FirstOrDefaultAsync(c => c.Id == id);

        if (status == null)
        {
            return new ActionResponse<Status>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Status>
        {
            WasSuccess = true,
            Result = status
        };
    }

    public async Task<IEnumerable<Status>> GetComboAsync()
    {
        return await _context.Statuses
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Status>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Statuses
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Status>>
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
        var queryable = _context.Statuses.AsQueryable();

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