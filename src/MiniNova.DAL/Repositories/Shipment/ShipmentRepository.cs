using Microsoft.EntityFrameworkCore;
using MiniNova.DAL.Context;
using MiniNova.DAL.Records;

namespace MiniNova.DAL.Repositories.Shipment;

public class ShipmentRepository : IShipmentRepository
{
    private readonly NovaDbContext _dbContext;
    
    public ShipmentRepository(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Models.Shipment?> GetByIdWithDetailsAsync(int shipmentId, CancellationToken cancellationToken)
    {
        var shipment = await _dbContext.Shipments
            .Include(d => d.Destination)
            .Include(d => d.Origin)
            .Include(s => s.Shipper)
            .Include(r => r.Consignee)
            .Include(p => p.Trackings)
            .ThenInclude(t => t.Operator).ThenInclude(o => o.Person)
            .Include(p => p.Trackings)
            .ThenInclude(t => t.Operator).ThenInclude(o => o.Occupation)
            .Include(p => p.Size)
            .Include(p => p.Trackings).ThenInclude(t => t.Status)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == shipmentId,  cancellationToken);

        return shipment;
    }

    public async Task<Models.Shipment?> GetByTrackNoAsync(string trackNo, CancellationToken cancellationToken)
    {
        var shipment = await _dbContext.Shipments
            .Include(d => d.Destination)
            .Include(d => d.Origin)
            .Include(s => s.Shipper)
            .Include(r => r.Consignee)
            .Include(p => p.Trackings)
            .ThenInclude(t => t.Operator).ThenInclude(o => o.Person)
            .Include(p => p.Trackings)
            .ThenInclude(t => t.Operator).ThenInclude(o => o.Occupation)
            .Include(p => p.Size)
            .Include(p => p.Trackings).ThenInclude(t => t.Status)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.TrackId == trackNo, cancellationToken);
        
        return shipment;
    }

    public async Task<PaginationResult<Models.Shipment>> GetPagedAsync(int skip, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.Shipments
            .AsNoTracking()
            .AsQueryable();
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Include(p => p.Shipper)
            .Include(p => p.Consignee)
            .Include(p => p.Origin)
            .Include(p => p.Destination)
            .Include(p => p.Trackings).ThenInclude(t => t.Status)
            .OrderByDescending(p => p.Id)
            .Skip(skip)
            .Take(pageSize)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
        
        return new PaginationResult<Models.Shipment>(items, totalCount);
    }

    public async Task<PaginationResult<Models.Shipment>> GetByUserIdPagedAsync(int userId, int skip, int take, CancellationToken cancellationToken)
    {
        var query = _dbContext.Shipments
            .AsNoTracking()
            .Where(p => p.ShipperId == userId || p.ConsigneeId == userId);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Include(p => p.Shipper)
            .Include(p => p.Consignee)
            .Include(p => p.Destination)
            .Include(p => p.Size)
            .Include(p => p.Trackings).ThenInclude(t => t.Status)
            .OrderByDescending(p => p.Id)
            .Skip(skip)
            .Take(take)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
           
        return new PaginationResult<Models.Shipment>(items, totalCount);
    }

    public async Task AddAsync(Models.Shipment shipment, CancellationToken cancellationToken)
    {
        await _dbContext.Shipments.AddAsync(shipment, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Models.Shipment shipment, CancellationToken cancellationToken)
    {
        _dbContext.Shipments.Update(shipment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Remove(Models.Shipment shipment, CancellationToken cancellationToken)
    {
        _dbContext.Shipments.Remove(shipment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<bool> ExistsAsync(int id, CancellationToken token)
    {
        return await _dbContext.Shipments.AnyAsync(s => s.Id == id, token);
    }
}