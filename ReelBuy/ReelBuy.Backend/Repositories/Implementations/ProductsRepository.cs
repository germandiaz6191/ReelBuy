using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Enums;
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
            .Include(p => p.Reels)
            .Include(p => p.Status)
            .Include(p => p.Category)
            .Include(p => p.Marketplace)
            .Include(p => p.Favorites)
            .Include(p => p.Store)
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
            .Include(p => p.Reels)
            .Include(p => p.Status)
            .Include(p => p.Category)
            .Include(p => p.Marketplace)
            .Include(p => p.Favorites)
            .Include(p => p.Store)
            .AsQueryable();

        if (pagination.FilterStatus.HasValue)
        {
            queryable = queryable.Where(x => x.StatusId == pagination.FilterStatus);
            
        }

        queryable = pagination.FilterStatus.HasValue && pagination.FilterStatus == (int)StatusProduct.Approved
                            ? queryable.OrderByDescending(x => x.LikesGroup)
                            : queryable.OrderBy(x => x.Name);

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }
        
        // Aplicar filtro por IDs de tiendas
        if (!string.IsNullOrWhiteSpace(pagination.StoreIds))
        {
            var storeIdList = pagination.StoreIds.Split(',')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(int.Parse)
                .ToList();
                
            if (storeIdList.Any())
            {
                queryable = queryable.Where(x => storeIdList.Contains(x.StoreId));
            }
        }

        return new ActionResponse<IEnumerable<Product>>
        {
            WasSuccess = true,
            Result = await queryable
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Products.AsQueryable();

        if (pagination.FilterStatus.HasValue)
        {
            queryable = queryable.Where(x => x.StatusId.Equals(pagination.FilterStatus));
        }

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }
        
        // Aplicar filtro por IDs de tiendas
        if (!string.IsNullOrWhiteSpace(pagination.StoreIds))
        {
            var storeIdList = pagination.StoreIds.Split(',')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(int.Parse)
                .ToList();
                
            if (storeIdList.Any())
            {
                queryable = queryable.Where(x => storeIdList.Contains(x.StoreId));
            }
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

        queryable = queryable.Where(x => x.StatusId == (int)StatusProduct.Approved);

        if (!string.IsNullOrWhiteSpace(principalSearch.keyword))
        {
            queryable = queryable.Where(x => EF.Functions.Like(x.Name, $"%{principalSearch.keyword}%"));
        }

        var results = await queryable
        .OrderByDescending(x => x.LikesGroup)
        .Take(11)
        .ToListAsync();

        return new ActionResponse<IEnumerable<Product>>
        {
            WasSuccess = true,
            Result = results
        };
    }

    public async Task<ActionResponse<int>> UpdateAsync(IEnumerable<Product> entities)
    {
        try
        {
            foreach (var product in entities)
            {
                _context.Entry(product).State = EntityState.Modified;
                _context.Entry(product).Reference(p => p.Store).IsModified = false;
                _context.Entry(product).Reference(p => p.Category).IsModified = false;
                _context.Entry(product).Reference(p => p.Store).IsModified = false;
                _context.Entry(product).Collection(p => p.Reels).IsModified = false;
                _context.Entry(product).Reference(p => p.Marketplace).IsModified = false;
            }

            _context.UpdateRange(entities);
            await _context.SaveChangesAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = entities.Count()
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<int>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<int>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }

    public override async Task<ActionResponse<Product>> UpdateAsync(Product entity)
    {
        try
        {
            var currentProduct = await _context.Products
                .Include(p => p.Reels)
                .Include(p => p.Status)
                .Include(p => p.Category)
                .Include(p => p.Marketplace)
                .Include(p => p.Favorites)
                .Include(p => p.Store)
                .FirstOrDefaultAsync(p => p.Id == entity.Id);

            if (currentProduct == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "ERR001"
                };
            }

            // Actualizar propiedades básicas
            currentProduct.Name = entity.Name;
            currentProduct.Description = entity.Description;
            currentProduct.Price = entity.Price;
            currentProduct.StatusId = entity.StatusId;
            currentProduct.CategoryId = entity.CategoryId;
            currentProduct.MarketplaceId = entity.MarketplaceId;
            currentProduct.StoreId = entity.StoreId;
            currentProduct.MotiveReject = entity.MotiveReject;

            // No modificar las relaciones
            _context.Entry(currentProduct).Reference(p => p.Store).IsModified = false;
            _context.Entry(currentProduct).Reference(p => p.Category).IsModified = false;
            _context.Entry(currentProduct).Reference(p => p.Marketplace).IsModified = false;
            _context.Entry(currentProduct).Collection(p => p.Reels).IsModified = false;
            _context.Entry(currentProduct).Collection(p => p.Favorites).IsModified = false;
            _context.Entry(currentProduct).Collection(p => p.Comments).IsModified = false;
            _context.Entry(currentProduct).Collection(p => p.LikedBy).IsModified = false;

            await _context.SaveChangesAsync();
            return new ActionResponse<Product>
            {
                WasSuccess = true,
                Result = currentProduct
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Product>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<Product>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }
}