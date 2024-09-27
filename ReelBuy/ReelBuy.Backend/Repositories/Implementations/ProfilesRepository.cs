using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Implementations
{
    public class ProfilesRepository : GenericRepository<Profiles>, IProfilesRepository
    {
        private readonly DataContext _context;

        public ProfilesRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResponse<IEnumerable<Profiles>>> GetAsync()
        {
            var profiles = await _context.Profiles
                .OrderBy(c => c.Name)
                .ToListAsync();
            return new ActionResponse<IEnumerable<Profiles>>
            {
                WasSuccess = true,
                Result = profiles
            };
        }

        public override async Task<ActionResponse<Profiles>> GetAsync(int id)
        {
            var profile = await _context.Profiles
                 .FirstOrDefaultAsync(c => c.Id == id);

            if (profile == null)
            {
                return new ActionResponse<Profiles>
                {
                    WasSuccess = false,
                    Message = "ERR001"
                };
            }

            return new ActionResponse<Profiles>
            {
                WasSuccess = true,
                Result = profile
            };
        }

        public async Task<IEnumerable<Profiles>> GetComboAsync()
        {
            return await _context.Profiles
                .OrderBy(c => c.Name)
            .ToListAsync();
        }

        public override async Task<ActionResponse<IEnumerable<Profiles>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Profiles
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<Profiles>>
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
            var queryable = _context.Profiles.AsQueryable();

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
