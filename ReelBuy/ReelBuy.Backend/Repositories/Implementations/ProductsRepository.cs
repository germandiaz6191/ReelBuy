using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Implementations;

public class ProductsRepository : GenericRepository<Product>, IProductsRepository
{
    private readonly DataContext _context;

    public ProductsRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Product>>> GetAsync()
    {
        var products = await _context.Products
            .OrderBy(c => c.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Product>>
        {
            WasSuccess = true,
            Result = products
        };
    }

    public override async Task<ActionResponse<Product>> GetAsync(int id)
    {
        var products = await _context.Products
             .FirstOrDefaultAsync(c => c.Id == id);

        if (products == null)
        {
            return new ActionResponse<Product>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Product>
        {
            WasSuccess = true,
            Result = products
        };
    }

    public async Task<IEnumerable<Product>> GetComboAsync()
    {
        return await _context.Products
            .OrderBy(c => c.Name)
        .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Products
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Product>>
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
        var queryable = _context.Products.AsQueryable();

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

    public async Task<ActionResponse<IEnumerable<Product>>> GetProductsByLikeAsync(PrincipalSearchDTO principalSearch)
    {
        var queryable = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(principalSearch.keyword))
        {
            queryable = queryable.Where(x => EF.Functions.Like(x.Name, $"%{principalSearch.keyword}%"));
        }

        var results = await queryable
        .OrderBy(x => x.Name)
        .Take(11)
        .ToListAsync();

        return new ActionResponse<IEnumerable<Product>>
        {
            WasSuccess = true,
            Result = results
        };
    }
}