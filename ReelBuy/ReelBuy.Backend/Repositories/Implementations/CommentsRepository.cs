using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Helpers;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Implementations;

public class CommentsRepository : GenericRepository<Comments>, ICommentsRepository
{
    private readonly DataContext _context;

    public CommentsRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<Comments>>> GetAsync()
    {
        var comments = await _context.Comments
            .OrderBy(c => c.RegistrationDate)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Comments>>
        {
            WasSuccess = true,
            Result = comments
        };
    }

    public override async Task<ActionResponse<Comments>> GetAsync(int id)
    {
        var comments = await _context.Comments
             .FirstOrDefaultAsync(c => c.Id == id);

        if (comments == null)
        {
            return new ActionResponse<Comments>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Comments>
        {
            WasSuccess = true,
            Result = comments
        };
    }

    public override async Task<ActionResponse<IEnumerable<Comments>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Comments
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.RegistrationDate.Equals(pagination.Filter));
        }

        return new ActionResponse<IEnumerable<Comments>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.RegistrationDate)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Comments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.RegistrationDate.Equals(pagination.Filter));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public async Task<ActionResponse<IEnumerable<Comments>>> GetCommentsByProductAsync(PaginationDTO pagination)
    {
        var queryable = _context.Comments
            .Include(c => c.User)
            .AsQueryable();

        queryable = queryable.Where(x => x.ProductId == int.Parse(pagination.Filter));

        return new ActionResponse<IEnumerable<Comments>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.RegistrationDate)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public async Task<ActionResponse<Comments>> AddAsync(CommetDTO comment)
    {
        var product = await _context.Products.FindAsync(comment.ProductId);
        if (product == null)
        {
            return new ActionResponse<Comments>
            {
                WasSuccess = false,
                Message = "ERR009"
            };
        }

        var user = await _context.Users.FindAsync(comment.UserId);
        if (user == null)
        {
            return new ActionResponse<Comments>
            {
                WasSuccess = false,
                Message = "ERR009"
            };
        }

        var currentComment = new Comments()
        {
            Product = product,
            ProductId = product.Id,
            User = user,
            UserId = user.Id,
            Description = comment.Description,
            RegistrationDate = comment.RegistrationDate
        };

        _context.Add(currentComment);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Comments>
            {
                WasSuccess = true,
                Result = currentComment
            };
        }
        catch (DbUpdateException e)
        {
            Console.Write(e);
            return new ActionResponse<Comments>
            {
                WasSuccess = false,
                Message = "ERR003"
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Comments>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}