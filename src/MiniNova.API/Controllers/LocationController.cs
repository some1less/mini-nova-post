using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;

namespace MiniNova.API.Controllers;

[Route("api/locations")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly NovaDbContext _context;

    public LocationController(NovaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Location>>> GetLocations(CancellationToken cancellationToken)
    {
        return await _context.Locations.ToListAsync(cancellationToken);
    }
}