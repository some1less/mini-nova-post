using MiniNova.BLL.DTO.Tracking;

namespace MiniNova.BLL.Services.Tracking;

public interface ITrackingService
{
    
    Task<IEnumerable<TrackingResponseDTO>> GetHistoryByPackageIdAsync(int packageId, CancellationToken cancellationToken);
    Task<TrackingResponseDTO> AddTrackingAsync(TrackingDTO trackingDto, string operatorEmail, CancellationToken cancellationToken);
    Task UpdateTrackingAsync(int trackingId, UpdateTrackingDTO trackingDto, CancellationToken cancellationToken);
    Task DeleteTrackingAsync(int trackingId, CancellationToken cancellationToken);
}