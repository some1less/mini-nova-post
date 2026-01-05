namespace MiniNova.BLL.DTO.Tracking;

public class TrackingResponseDTO
{
    public int Id { get; set; }
    public required string Status { get; set; }
    public required string UpdateTime { get; set; }
    
    
    public required string OperatorName { get; set; } 
    public required string OperatorRole { get; set; }
}