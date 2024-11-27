using Microsoft.EntityFrameworkCore;
using Orders.Backend.Helpers;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Implementations;

public class DepartmentsRepository : GenericRepository<Department>, IDepartmentsRepository
{
    private readonly DataContext _context;

    public DepartmentsRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Department>>> GetAsync()
    {
        var departments = await _context.Departments
            .OrderBy(c => c.Name)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Department>>
        {
            WasSuccess = true,
            Result = departments
        };
    }

    public override async Task<ActionResponse<Department>> GetAsync(int id)
    {
        var department = await _context.Departments
             .FirstOrDefaultAsync(c => c.Id == id);

        if (department == null)
        {
            return new ActionResponse<Department>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Department>
        {
            WasSuccess = true,
            Result = department
        };
    }

    public async Task<IEnumerable<Department>> GetComboAsync()
    {
        return await _context.Departments
            .OrderBy(c => c.Name)
        .ToListAsync();
    }

    public override async Task<ActionResponse<IEnumerable<Department>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Departments
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Department>>
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
        var queryable = _context.Departments.AsQueryable();

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

    public async Task<ActionResponse<Department>> AddAsync(DepartmentDTO departmentDTO)
    {
        var country = await _context.Countries.FindAsync(departmentDTO.CountryId);
        if (country == null)
        {
            return new ActionResponse<Department>
            {
                WasSuccess = false,
                Message = "ERR009"
            };
        }

        var department = departmentDTO.Department;
        if (department == null)
        {
            return new ActionResponse<Department>
            {
                WasSuccess = false,
                Message = "ERR005"
            };
        }

        department.Country = country;

        _context.Add(department);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Department>
            {
                WasSuccess = true,
                Result = department
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Department>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Department>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    public async Task<ActionResponse<Department>> UpdateAsync(DepartmentDTO departmentDTO)
    {
        var department = departmentDTO?.Department;
        var currentDepartment = await _context.Departments.FindAsync(department?.Id);
        if (currentDepartment == null || department == null)
        {
            return new ActionResponse<Department>
            {
                WasSuccess = false,
                Message = "ERR005"
            };
        }

        var country = await _context.Countries.FindAsync(departmentDTO?.CountryId);
        if (country == null)
        {
            return new ActionResponse<Department>
            {
                WasSuccess = false,
                Message = "ERR004"
            };
        }

        currentDepartment.Name = department.Name;
        currentDepartment.Country = country;

        _context.Update(currentDepartment);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Department>
            {
                WasSuccess = true,
                Result = currentDepartment
            };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Department>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Department>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}