using MiniNova.DAL.Models;
using MiniNova.DAL.Records;

namespace MiniNova.DAL.Repositories.Interfaces;

public interface IShipmentRepository
{
    
    Task<Shipment?> GetByIdWithDetailsAsync(int shipmentId, CancellationToken cancellationToken);
    Task<Shipment?> GetByTrackNoAsync(string trackNo, CancellationToken cancellationToken);
    
    Task<PaginationResult<Shipment>> GetPagedAsync(int skip, int pageSize, CancellationToken cancellationToken);
    Task<PaginationResult<Shipment>> GetByUserIdPagedAsync(int userId, int skip, int take, CancellationToken cancellationToken);
    
    Task AddAsync(Shipment shipment, CancellationToken cancellationToken);
    void Update(Shipment shipment);
    void Remove(Shipment shipment);
    
    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int id, CancellationToken token);
}