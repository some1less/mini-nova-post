namespace MiniNova.DAL.Models;

public class Tracking
{
    public int Id { get; set; }
    
    public int ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = null!;
    public int? OperatorId { get; set; }
    public Operator? Operator { get; set; }
    
    public int StatusId { get; set; } 
    public Status Status { get; set; } = null!;
    
    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    
}