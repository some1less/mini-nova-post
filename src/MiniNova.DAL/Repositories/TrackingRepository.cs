using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;
using MiniNova.DAL.Repositories.Interfaces;

namespace MiniNova.DAL.Repositories;

public class TrackingRepository : ITrackingRepository
{
    private readonly NovaDbContext _dbContext;

    public TrackingRepository(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Tracking>> GetAllAsync(int shipmentId, CancellationToken cancellationToken)
    {
        var history = await _dbContext.Trackings
            .AsNoTracking()
            .Where(t => t.ShipmentId == shipmentId)
            .Include(t => t.Status) 
            .Include(t => t.Operator)
            .ThenInclude(o => o!.Person)
            .Include(t => t.Operator)
            .ThenInclude(o => o!.Occupation)
            .OrderByDescending(t => t.UpdateTime)
            .ToListAsync(cancellationToken);
        
        return history;
    }

    public async Task<Tracking?> GetByIdAsync(int trackingId, CancellationToken cancellationToken)
    {
        var tracking = await _dbContext.Trackings
            .FirstOrDefaultAsync(t => t.Id == trackingId, cancellationToken);
        
        return tracking;
            
    }

    public async Task<Tracking?> GetByIdReadOnlyAsync(int trackingId, CancellationToken cancellationToken)
    {
        return await _dbContext.Trackings
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == trackingId, cancellationToken);
        
    }

    public async Task AddAsync(Tracking tracking, CancellationToken cancellationToken)
    {
        await _dbContext.Trackings.AddAsync(tracking, cancellationToken);
    }

    public void Update(Tracking tracking)
    {
        _dbContext.Trackings.Update(tracking);
    }

    public void Delete(Tracking tracking)
    {
        _dbContext.Trackings.Remove(tracking);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}