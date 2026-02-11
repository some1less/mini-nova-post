using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Context;
namespace MiniNova.DAL.Repositories.Location;

public class LocationRepository : ILocationRepository
{
    private readonly NovaDbContext _dbContext;

    public LocationRepository(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Models.Location?> GetByIdAsync(int locationId, CancellationToken ct)
    {
        var location = await _dbContext.Locations.FirstOrDefaultAsync(l => l.Id == locationId, ct);
        return location;
    }
}