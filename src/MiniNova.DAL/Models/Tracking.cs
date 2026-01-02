namespace MiniNova.DAL.Models;

public class Tracking
{
    public int Id { get; set; }
    
    public int PackageId { get; set; }
    public Package Package { get; set; } = null!;
    public int? OperatorId { get; set; }
    public Operator? Operator { get; set; }
    
    public required string Status { get; set; } 
    
    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    
}