namespace MiniNova.BLL.DTO.Tracking;

public class UpdateTrackingDTO
{
    public int PackageId { get; set; }
    public required string Status { get; set; }
}