using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;
using MiniNova.DAL.Repositories.Interfaces;

namespace MiniNova.DAL.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly NovaDbContext _dbContext;

    public LocationRepository(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Location?> GetByIdAsync(int locationId, CancellationToken cancellationToken)
    {
        var location = await _dbContext.Locations.FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);
        return location;
    }
}