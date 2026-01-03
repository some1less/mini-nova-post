namespace MiniNova.BLL.DTO.Tracking;

public class TrackingResponseDTO
{
    public int Id { get; set; }
    public string Status { get; set; }
    public string UpdateTime { get; set; }
    
    
    public string OperatorName { get; set; } 
    public string OperatorRole { get; set; }
}