using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Implementations
{
    public class CategoriesRepository : GenericRepository<Categories>, ICategoriesRepository
    {
        private readonly DataContext _context;

        public CategoriesRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResponse<IEnumerable<Categories>>> GetAsync()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
            return new ActionResponse<IEnumerable<Categories>>
            {
                WasSuccess = true,
                Result = categories
            };
        }

        public override async Task<ActionResponse<Categories>> GetAsync(int id)
        {
            var categories = await _context.Categories
                 .FirstOrDefaultAsync(c => c.Id == id);

            if (categories == null)
            {
                return new ActionResponse<Categories>
                {
                    WasSuccess = false,
                    Message = "ERR001"
                };
            }

            return new ActionResponse<Categories>
            {
                WasSuccess = true,
                Result = categories
            };
        }

        public async Task<IEnumerable<Categories>> GetComboAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
            .ToListAsync();
        }

        public override async Task<ActionResponse<IEnumerable<Categories>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Categories
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<Categories>>
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
            var queryable = _context.Categories.AsQueryable();

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
}
