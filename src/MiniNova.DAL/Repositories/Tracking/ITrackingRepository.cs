using MiniNova.DAL.Models;

namespace MiniNova.DAL.Repositories.Tracking;

public interface ITrackingRepository
{
    Task<List<Models.Tracking>> GetAllAsync(int shipmentId, CancellationToken cancellationToken);
    Task<Models.Tracking?> GetByIdAsync(int trackingId, CancellationToken cancellationToken);
    Task<Models.Tracking?> GetByIdReadOnlyAsync(int trackingId, CancellationToken cancellationToken);
    
    Task AddAsync(Models.Tracking tracking, CancellationToken cancellationToken);
    void Update(Models.Tracking tracking);
    void Delete(Models.Tracking tracking);
    Task SaveChangesAsync(CancellationToken cancellationToken);

}