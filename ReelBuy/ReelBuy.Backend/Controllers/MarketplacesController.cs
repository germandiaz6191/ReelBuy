using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Shared.Entities;

namespace ReelBuy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketplacesController : ControllerBase
{
    private readonly DataContext _context;

    public MarketplacesController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        return Ok(await _context.Marketplaces.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id)
    {
        var marketplace = await _context.Marketplaces.FindAsync(id);
        if (marketplace == null)
        {
            return NotFound();
        }
        return Ok(marketplace);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(Marketplace marketplace)
    {
        _context.Add(marketplace);
        await _context.SaveChangesAsync();
        return Ok(marketplace);
    }

    [HttpPut]
    public async Task<IActionResult> PutAsync(Marketplace marketplace)
    {
        var currentMarketplace = await _context.Marketplaces.FindAsync(marketplace.Id);
        if (currentMarketplace == null)
        {
            return NotFound();
        }
        currentMarketplace.Name = marketplace.Name;
        _context.Update(currentMarketplace);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var marketPlace = await _context.Marketplaces.FindAsync(id);
        if (marketPlace == null)
        {
            return NotFound();
        }
        _context.Remove(marketPlace);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}