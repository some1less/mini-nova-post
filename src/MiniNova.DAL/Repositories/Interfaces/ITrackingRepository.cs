using MiniNova.DAL.Models;

namespace MiniNova.DAL.Repositories.Interfaces;

public interface ITrackingRepository
{
    Task<List<Tracking>> GetAllAsync(int shipmentId, CancellationToken cancellationToken);
    Task<Tracking?> GetByIdAsync(int trackingId, CancellationToken cancellationToken);
    Task<Tracking?> GetByIdReadOnlyAsync(int trackingId, CancellationToken cancellationToken);
    
    Task AddAsync(Tracking tracking, CancellationToken cancellationToken);
    void Update(Tracking tracking);
    void Delete(Tracking tracking);
    Task SaveChangesAsync(CancellationToken cancellationToken);

}