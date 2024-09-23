using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Shared.Entities;

namespace ReelBuy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusesController : ControllerBase
{
    private readonly DataContext _context;

    public StatusesController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        return Ok(await _context.Statuses.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id)
    {
        var status = await _context.Statuses.FindAsync(id);
        if (status == null)
        {
            return NotFound();
        }
        return Ok(status);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(Status status)
    {
        _context.Add(status);
        await _context.SaveChangesAsync();
        return Ok(status);
    }

    [HttpPut]
    public async Task<IActionResult> PutAsync(Status status)
    {
        var currentStatus = await _context.Statuses.FindAsync(status.Id);
        if (currentStatus == null)
        {
            return NotFound();
        }
        currentStatus.Name = status.Name;
        _context.Update(currentStatus);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var status = await _context.Statuses.FindAsync(id);
        if (status == null)
        {
            return NotFound();
        }
        _context.Remove(status);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}