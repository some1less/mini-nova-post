using MiniNova.BLL.DTO.Tracking;
using MiniNova.DAL.Models;

namespace MiniNova.BLL.Mappers;

public static class TrackingMapper
{
    public static TrackingResponseDTO ToDto(this Tracking t)
    {
        return new TrackingResponseDTO
        {
            Id = t.Id,
            Status = t.Status?.Name ?? "Unknown",
            UpdateTime = t.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            OperatorName = t.Operator != null
                ? $"{t.Operator.Person.FirstName} {t.Operator.Person.LastName}"
                : "System Auto-Update",
            OperatorRole = t.Operator?.Occupation.Name ?? "System"
        };
    }
    
    public static Tracking ToEntity(this TrackingDTO dto, int operatorId)
    {
        return new Tracking
        {
            ShipmentId = dto.PackageId,
            OperatorId = operatorId,
            StatusId = dto.StatusId,
            UpdateTime = DateTime.UtcNow,
        };
    }
    
    public static void MapUpdate(this Tracking tracking, UpdateTrackingDTO dto)
    {
        tracking.StatusId = dto.StatusId;
    }
}