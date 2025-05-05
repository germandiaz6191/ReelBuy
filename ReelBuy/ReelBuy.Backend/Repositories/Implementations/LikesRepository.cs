using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.Repositories.Implementations;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<ActionResponse<bool>> GetAsync(string userId, int productId)
    {
        var product = await _context.Users
                                .Where(u => u.Id == userId)
                                .SelectMany(u => u.Likes)
                                .AnyAsync(p => p.Id == productId);

        return new ActionResponse<bool>
        {
            WasSuccess = true,
            Result = product
        };
    }

    public async Task<ActionResponse<int>> AddAsync(LikeDTO dto)
    {
        try
        {
            var product = await _context.Products
                                    .Include(p => p.LikedBy)
                                    .FirstOrDefaultAsync(p => p.Id == dto.ProductId);
            if (product == null)
            {
                return new ActionResponse<int>
                {
                    WasSuccess = false,
                    Message = "ERR009"
                };
            }

            var user = await _context.Users
                            .Include(u => u.Likes)
                            .FirstOrDefaultAsync(u => u.Id == dto.UserId);

            if (user == null)
            {
                return new ActionResponse<int>
                {
                    WasSuccess = false,
                    Message = "ERR009"
                };
            }

            int affectedRows = await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE Products SET LikesGroup = LikesGroup + 1 WHERE Id = {dto.ProductId}");

            product.LikedBy?.Add(user);
            await _context.SaveChangesAsync();

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = affectedRows
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

    public async Task<ActionResponse<int>> DeleteAsync(string userId, int productId)
    {
        try
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return new ActionResponse<int>
                {
                    WasSuccess = false,
                    Message = "ERR009"
                };
            }

            var user = await _context.Users
                .Include(u => u.Likes)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return new ActionResponse<int>
                {
                    WasSuccess = false,
                    Message = "ERR009"
                };
            }

            int affectedRows = await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE Products SET LikesGroup = LikesGroup - 1 WHERE Id = {productId}");

            user.Likes.Remove(product);
            await _context.SaveChangesAsync();

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = affectedRows
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
}