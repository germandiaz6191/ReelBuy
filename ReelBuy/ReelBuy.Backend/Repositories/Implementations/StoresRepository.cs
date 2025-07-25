﻿using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Implementations;

public class StoresRepository : GenericRepository<Store>, IStoresRepository
{
    private readonly DataContext _context;

    public StoresRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Store>>> GetAsync()
    {
        var stores = await _context.Stores
            .OrderBy(c => c.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Store>>
        {
            WasSuccess = true,
            Result = stores
        };
    }

    public override async Task<ActionResponse<Store>> GetAsync(int id)
    {
        var store = await _context.Stores
             .FirstOrDefaultAsync(c => c.Id == id);

        if (store == null)
        {
            return new ActionResponse<Store>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Store>
        {
            WasSuccess = true,
            Result = store
        };
    }

    public async Task<IEnumerable<Store>> GetComboAsync()
    {
        return await _context.Stores
            .OrderBy(c => c.Name)
        .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Store>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Stores
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }
        
        if (!string.IsNullOrEmpty(pagination.UserId))
        {
            queryable = queryable.Where(x => x.UserId == pagination.UserId);
        }

        return new ActionResponse<IEnumerable<Store>>
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
        var queryable = _context.Stores.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }
        
        if (!string.IsNullOrEmpty(pagination.UserId))
        {
            queryable = queryable.Where(x => x.UserId == pagination.UserId);
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public async Task<ActionResponse<Store>> AddAsync(StoreDTO storeDTO)
    {
        var city = await _context.Cities.FindAsync(storeDTO.CityId);
        if (city == null)
        {
            return new ActionResponse<Store>
            {
                WasSuccess = false,
                Message = "ERR009"
            };
        }

        var store = new Store()
        {
            City = city,
            CityId = city.Id,
            Name = storeDTO.Name,
            UserId = storeDTO.UserId,
        };

        _context.Add(store);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Store>
            {
                WasSuccess = true,
                Result = store
            };
        }
        catch (DbUpdateException dbE) 
        {
            return new ActionResponse<Store>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Store>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    public async Task<ActionResponse<Store>> UpdateAsync(StoreDTO storeDTO)
    {
        var currentStore = await _context.Stores.FindAsync(storeDTO?.Id);
        if (currentStore == null)
        {
            return new ActionResponse<Store>
            {
                WasSuccess = false,
                Message = "ERR005"
            };
        }

        var city = await _context.Cities.FindAsync(storeDTO?.CityId);
        if (city == null)
        {
            return new ActionResponse<Store>
            {
                WasSuccess = false,
                Message = "ERR004"
            };
        }

        currentStore.Name = storeDTO!.Name;
        currentStore.City = city;

        _context.Update(currentStore);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Store>
            {
                WasSuccess = true,
                Result = currentStore
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Store>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Store>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    public async Task<ActionResponse<IEnumerable<Store>>> GetStoresByUserAsync(string userId)
    {
        var stores = await _context.Stores
            .Include(s => s.Products)
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return new ActionResponse<IEnumerable<Store>>
        {
            WasSuccess = true,
            Result = stores
        };
    }
}