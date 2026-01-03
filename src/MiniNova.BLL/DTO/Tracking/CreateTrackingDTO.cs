namespace MiniNova.BLL.DTO.Tracking;

public class CreateTrackingDTO
{
    public int PackageId { get; set; }
    public required string Status { get; set; }
}