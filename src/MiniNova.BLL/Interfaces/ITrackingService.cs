using MiniNova.BLL.DTO.Tracking;

namespace MiniNova.BLL.Interfaces;

public interface ITrackingService
{
    
    Task<IEnumerable<TrackingResponseDTO>> GetHistoryByPackageIdAsync(int packageId);
    Task<TrackingResponseDTO> AddTrackingAsync(CreateTrackingDTO trackingDto, string operatorEmail);
    Task UpdateTrackingAsync(int trackingId, UpdateTrackingDTO trackingDto);
    Task DeleteTrackingAsync(int trackingId);
}