using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Implementations;

public class CitiesRepository : GenericRepository<City>, ICitiesRepository
{
    private readonly DataContext _context;

    public CitiesRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<City>>> GetAsync()
    {
        var cities = await _context.Cities
            .OrderBy(c => c.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<City>>
        {
            WasSuccess = true,
            Result = cities
        };
    }

    public override async Task<ActionResponse<City>> GetAsync(int id)
    {
        var city = await _context.Cities
             .FirstOrDefaultAsync(c => c.Id == id);

        if (city == null)
        {
            return new ActionResponse<City>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<City>
        {
            WasSuccess = true,
            Result = city
        };
    }

    public async Task<IEnumerable<City>> GetComboAsync()
    {
        return await _context.Cities
            .OrderBy(c => c.Name)
        .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<City>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Cities
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<City>>
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
        var queryable = _context.Cities.AsQueryable();

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

    public async Task<ActionResponse<City>> AddAsync(CityDTO cityDTO)
    {
        var department = await _context.Departments.FindAsync(cityDTO.DepartmentId);
        if (department == null)
        {
            return new ActionResponse<City>
            {
                WasSuccess = false,
                Message = "ERR009"
            };
        }

        var city = new City()
        {
            Department = department,
            DepartmentId = department.Id,
            Name = cityDTO.Name

        };

        _context.Add(city);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<City>
            {
                WasSuccess = true,
                Result = city
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<City>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<City>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    public async Task<ActionResponse<City>> UpdateAsync(CityDTO cityDTO)
    {
        var currentCity = await _context.Cities.FindAsync(cityDTO?.Id);
        if (currentCity == null)
        {
            return new ActionResponse<City>
            {
                WasSuccess = false,
                Message = "ERR005"
            };
        }

        var department = await _context.Departments.FindAsync(cityDTO?.DepartmentId);
        if (department == null)
        {
            return new ActionResponse<City>
            {
                WasSuccess = false,
                Message = "ERR004"
            };
        }

        currentCity.Name = cityDTO!.Name;
        currentCity.Department = department;

        _context.Update(currentCity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<City>
            {
                WasSuccess = true,
                Result = currentCity
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<City>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<City>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}