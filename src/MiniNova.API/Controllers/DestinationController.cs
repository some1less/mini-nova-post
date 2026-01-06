using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Context;

namespace MiniNova.API.Controllers;

[Route("api/destinations")]
[ApiController]
public class DestinationController : ControllerBase
{
    private readonly NovaDbContext _context;

    public DestinationController(NovaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetDestinations()
    {
        var list = await _context.Destinations
            .Select(d => new { d.Id, Address = $"{d.City}, {d.Street}" })
            .ToListAsync();
        
        return Ok(list);
    }
}