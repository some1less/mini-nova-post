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


    public async Task<IEnumerable<TrackingResponseDTO>> GetHistoryByPackageIdAsync(int packageId)
    {
        var package = await _dbContext.Packages
            .FirstOrDefaultAsync(p => p.Id == packageId);
        if (package == null) throw new KeyNotFoundException($"Package with id {packageId} not found");
        
        var history = await _dbContext.Trackings
            .Where(t => t.PackageId == packageId)
            .Include(o => o.Operator)
            .ThenInclude(p => p.Person)
            .Include(o => o.Operator)
            .ThenInclude(o => o.Occupation)
            .OrderByDescending(t => t.UpdateTime)
            .ToListAsync();

        return history.Select(t => new TrackingResponseDTO()
        {
            Id = t.Id,
            Status = t.Status,
            UpdateTime = t.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            OperatorName = t.Operator != null
                ? $"{t.Operator.Person.FirstName} {t.Operator.Person.LastName}"
                : "System Auto-Update",
            OperatorRole = t.Operator?.Occupation.Name ?? "System"
        });
    }

    public async Task<TrackingResponseDTO> AddTrackingAsync(CreateTrackingDTO trackingDto, string operatorLogin)
    {
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Login == operatorLogin);
        if (account == null) throw new KeyNotFoundException($"Account with login {operatorLogin} not found");
        
        var oper = await _dbContext.Operators
            .Include(o => o.Occupation)
            .Include(p => p.Person)
            .FirstOrDefaultAsync(p => p.PersonId == account.PersonId);

        if (oper == null) throw new UnauthorizedAccessException("Current user is not an Operator");

        var tracking = new Tracking
        {
            PackageId = trackingDto.PackageId,
            OperatorId = oper.Id,
            Status = trackingDto.Status,
            UpdateTime = DateTime.UtcNow,
        };
        
        await  _dbContext.Trackings.AddAsync(tracking);
        await _dbContext.SaveChangesAsync();
        
        return new TrackingResponseDTO
        {
            Id = tracking.Id,
            Status = tracking.Status,
            UpdateTime = tracking.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            OperatorName = $"{oper.Person.FirstName} {oper.Person.LastName}",
            OperatorRole = oper.Occupation.Name
        };
    }

    public async Task UpdateTrackingAsync(int trackingId, UpdateTrackingDTO trackingDto)
    {
        var tracking = await _dbContext.Trackings.FirstOrDefaultAsync(t => t.Id == trackingId);
        
        if (tracking == null) 
            throw new KeyNotFoundException($"Tracking record with id {trackingId} not found");

        tracking.Status = trackingDto.Status;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteTrackingAsync(int trackingId)
    {
        var tracking = await _dbContext.Trackings.FirstOrDefaultAsync(t => t.Id == trackingId);
        
        if (tracking == null) 
            throw new KeyNotFoundException($"Tracking record with id {trackingId} not found");

        _dbContext.Trackings.Remove(tracking);
        await _dbContext.SaveChangesAsync();    
    }
}