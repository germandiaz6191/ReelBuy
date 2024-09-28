using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;
using System.Diagnostics.Metrics;

namespace ReelBuy.Backend.Repositories.Implementations;

public class MarketplacesRepository : GenericRepository<Marketplace>, IMarketplacesRepository
{
    private readonly DataContext _context;

    public MarketplacesRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Marketplace>>> GetAsync()
    {
        var marketplaces = await _context.Marketplaces
            .OrderBy(c => c.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Marketplace>>
        {
            WasSuccess = true,
            Result = marketplaces
        };
    }

    public override async Task<ActionResponse<Marketplace>> GetAsync(int id)
    {
        var marketplace = await _context.Marketplaces
             .FirstOrDefaultAsync(c => c.Id == id);

        if (marketplace == null)
        {
            return new ActionResponse<Marketplace>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Marketplace>
        {
            WasSuccess = true,
            Result = marketplace
        };
    }

    public async Task<IEnumerable<Marketplace>> GetComboAsync()
    {
        return await _context.Marketplaces
            .OrderBy(c => c.Name)
        .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Marketplace>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Marketplaces
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Marketplace>>
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
        var queryable = _context.Marketplaces.AsQueryable();

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