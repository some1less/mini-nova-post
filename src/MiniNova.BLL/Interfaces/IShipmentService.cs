using MiniNova.BLL.DTO.Package;
using MiniNova.BLL.Pagination;

namespace MiniNova.BLL.Interfaces;

public interface IShipmentService
{
    Task<PagedResponse<ShipmentAllDTO>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int pageSize = 10);
    Task<ShipmentByIdDTO> GetShipmentByIdAsync(int packageId, CancellationToken cancellationToken);
    //
    Task<ShipmentByIdDTO> CreateShipmentAsync(CreateShipmentDTO packageDto, CancellationToken cancellationToken, int senderId);
    Task UpdateShipmentAsync(UpdateShipmentDTO package, int packageId, CancellationToken cancellationToken);
    Task DeleteShipmentAsync(int packageId, CancellationToken cancellationToken);
    Task<PagedResponse<ShipmentByIdDTO>> GetUserShipmentsAsync(int userId, CancellationToken cancellationToken, int page, int pageSize);
    Task<ShipmentByIdDTO> GetShipmentByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken);
}