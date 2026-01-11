using Microsoft.EntityFrameworkCore;
using MiniNova.BLL.DTO.Tracking;
using MiniNova.BLL.Interfaces;
using MiniNova.DAL.Context;
using MiniNova.DAL.Models;

namespace MiniNova.BLL.Services;

public class TrackingService : ITrackingService
{
    
    private readonly NovaDbContext _dbContext;

    public TrackingService(NovaDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<IEnumerable<TrackingResponseDTO>> GetHistoryByPackageIdAsync(int packageId, CancellationToken cancellationToken)
    {
        var package = await _dbContext.Shipments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == packageId, cancellationToken);
        if (package == null) throw new KeyNotFoundException($"Package with id {packageId} not found");
        
        var history = await _dbContext.Trackings
            .AsNoTracking()
            .Where(t => t.ShipmentId == packageId)
            .Include(t => t.Status) 
            .Include(t => t.Operator)
            .ThenInclude(o => o!.Person)
            .Include(t => t.Operator)
            .ThenInclude(o => o!.Occupation)
            .OrderByDescending(t => t.UpdateTime)
            .ToListAsync(cancellationToken);

        return history.Select(t => new TrackingResponseDTO()
        {
            Id = t.Id,
            Status = t.Status?.Name ?? "Unknown",
            UpdateTime = t.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            OperatorName = t.Operator != null
                ? $"{t.Operator.Person.FirstName} {t.Operator.Person.LastName}"
                : "System Auto-Update",
            OperatorRole = t.Operator?.Occupation.Name ?? "System"
        });
    }

    public async Task<TrackingResponseDTO> AddTrackingAsync(TrackingDTO trackingDto, string operatorLogin, CancellationToken cancellationToken)
    {
        var package = await _dbContext.Shipments.FirstOrDefaultAsync(p => p.Id == trackingDto.PackageId,  cancellationToken);
        if (package == null) throw new KeyNotFoundException($"Package with id {trackingDto.PackageId} not found");
        
        var statusEntity = await _dbContext.Statuses.FirstOrDefaultAsync(s => s.Id == trackingDto.StatusId, cancellationToken);
        if (statusEntity == null) throw new KeyNotFoundException($"Status ID {trackingDto.StatusId} not found");

        var account = await _dbContext.Accounts
            .AsNoTracking()
            .Include(a => a.Person)
            .FirstOrDefaultAsync(a => a.Login == operatorLogin, cancellationToken);
            
        if (account == null) throw new KeyNotFoundException($"Account {operatorLogin} not found");

        var oper = await _dbContext.Operators
            .Include(o => o.Occupation)
            .Include(o => o.Person)
            .FirstOrDefaultAsync(o => o.PersonId == account.PersonId,  cancellationToken);

        if (oper == null) throw new UnauthorizedAccessException("Current user is not an Operator");
        
        var tracking = new Tracking
        {
            ShipmentId = trackingDto.PackageId,
            OperatorId = oper.Id,
            StatusId = trackingDto.StatusId,
            UpdateTime = DateTime.UtcNow,
        };
        
        await  _dbContext.Trackings.AddAsync(tracking, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new TrackingResponseDTO
        {
            Id = tracking.Id,
            Status = statusEntity.Name,
            UpdateTime = tracking.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            OperatorName = $"{oper.Person.FirstName} {oper.Person.LastName}",
            OperatorRole = oper.Occupation.Name
        };
    }

    public async Task UpdateTrackingAsync(int trackingId, UpdateTrackingDTO trackingDto, CancellationToken cancellationToken)
    {
        var tracking = await _dbContext.Trackings.FirstOrDefaultAsync(t => t.Id == trackingId,  cancellationToken);
        if (tracking == null) throw new KeyNotFoundException($"Tracking {trackingId} not found");

        var statusExists = await _dbContext.Statuses.AnyAsync(s => s.Id == trackingDto.StatusId, cancellationToken);
        if (!statusExists) throw new KeyNotFoundException($"Status ID {trackingDto.StatusId} not found");

        tracking.StatusId = trackingDto.StatusId;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTrackingAsync(int trackingId, CancellationToken cancellationToken)
    {
        var tracking = await _dbContext.Trackings.FirstOrDefaultAsync(t => t.Id == trackingId, cancellationToken);
        
        if (tracking == null) 
            throw new KeyNotFoundException($"Tracking record with id {trackingId} not found");

        _dbContext.Trackings.Remove(tracking);
        await _dbContext.SaveChangesAsync(cancellationToken);    
    }
}