using Microsoft.EntityFrameworkCore;
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
}