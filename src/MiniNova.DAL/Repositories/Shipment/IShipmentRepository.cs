using MiniNova.DAL.Models;
using MiniNova.DAL.Records;

namespace MiniNova.DAL.Repositories.Shipment;

public interface IShipmentRepository
{
    
    Task<Models.Shipment?> GetByIdWithDetailsAsync(int shipmentId, CancellationToken cancellationToken);
    Task<Models.Shipment?> GetByTrackNoAsync(string trackNo, CancellationToken cancellationToken);
    
    Task<PaginationResult<Models.Shipment>> GetPagedAsync(int skip, int pageSize, CancellationToken cancellationToken);
    Task<PaginationResult<Models.Shipment>> GetByUserIdPagedAsync(int userId, int skip, int take, CancellationToken cancellationToken);
    
    Task AddAsync(Models.Shipment shipment, CancellationToken cancellationToken);
    void Update(Models.Shipment shipment);
    void Remove(Models.Shipment shipment);
    
    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int id, CancellationToken token);
}