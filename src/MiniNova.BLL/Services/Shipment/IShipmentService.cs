using MiniNova.BLL.DTO.Shipment;
using MiniNova.BLL.Pagination;

namespace MiniNova.BLL.Services.Shipment;

public interface IShipmentService
{
    Task<PagedResponse<ShipmentAllDTO>> GetAllAsync(CancellationToken cancellationToken, int page = 1, int pageSize = 10);
    Task<ShipmentByIdDTO?> GetShipmentByIdAsync(int shipmentId, CancellationToken cancellationToken);
    //
    Task<ShipmentByIdDTO?> CreateShipmentAsync(CreateShipmentDTO shipmentDto, CancellationToken cancellationToken, int senderId);
    Task UpdateShipmentAsync(UpdateShipmentDTO shipmentDto, int shipmentId, CancellationToken cancellationToken);
    Task DeleteShipmentAsync(int shipmentId, CancellationToken cancellationToken);
    Task<PagedResponse<ShipmentByIdDTO>> GetUserShipmentsAsync(int userId, CancellationToken cancellationToken, int page, int pageSize);
    Task<ShipmentByIdDTO> GetShipmentByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken);
}