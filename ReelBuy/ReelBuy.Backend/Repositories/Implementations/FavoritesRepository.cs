using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Implementations;

public class FavoritesRepository : GenericRepository<Favorite>, IFavoritesRepository
{
    private readonly DataContext _context;

    public FavoritesRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Favorite>>> GetAsync()
    {
        var favorites = await _context.Favorites
            .OrderBy(c => c.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Favorite>>
        {
            WasSuccess = true,
            Result = favorites
        };
    }

    public override async Task<ActionResponse<Favorite>> GetAsync(int id)
    {
        var favorite = await _context.Favorites
             .FirstOrDefaultAsync(c => c.Id == id);

        if (favorite == null)
        {
            return new ActionResponse<Favorite>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Favorite>
        {
            WasSuccess = true,
            Result = favorite
        };
    }

    public async Task<ActionResponse<Favorite>> GetAsync(string userId, int productId)
    {
        var product = await _context.Favorites
                                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

        return new ActionResponse<Favorite>
        {
            WasSuccess = true,
            Result = product ?? new Favorite()
        };
    }

    public async Task<IEnumerable<Favorite>> GetComboAsync()
    {
        return await _context.Favorites
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Favorite>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Favorites
            .Include(p => p.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Favorite>>
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
        var queryable = _context.Favorites.AsQueryable();

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

    public async Task<ActionResponse<Favorite>> AddAsync(FavoriteDTO favoriteDTO)
    {
        var product = await _context.Products.FindAsync(favoriteDTO.ProductId);
        if (product == null)
        {
            return new ActionResponse<Favorite>
            {
                WasSuccess = false,
                Message = "ERR009"
            };
        }

        var user = await _context.Users.FindAsync(favoriteDTO.UserId);
        if (user == null)
        {
            return new ActionResponse<Favorite>
            {
                WasSuccess = false,
                Message = "ERR009"
            };
        }

        var favorite = new Favorite()
        {
            Product = product,
            ProductId = product.Id,
            User = user,
            UserId = user.Id
        };

        _context.Add(favorite);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Favorite>
            {
                WasSuccess = true,
                Result = favorite
            };
        }
        catch (DbUpdateException e)
        {
            Console.Write(e);
            return new ActionResponse<Favorite>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Favorite>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}