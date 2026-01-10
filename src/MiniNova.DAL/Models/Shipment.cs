namespace MiniNova.DAL.Models;

public class Shipment
{
    public int Id { get; set; }
    
    public required string TrackId { get; set; }
    
    public int ShipperId { get; set; }
    public Person Shipper { get; set; } = null!;
    public int ConsigneeId { get; set; }
    public Person Consignee { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public int SizeId { get; set; }
    public Size Size { get; set; } = null!;
    public decimal Weight { get; set; }
    
    public int DestinationId { get; set; }
    public Location Destination { get; set; } = null!;
    
    public int OriginId { get; set; }
    public Location Origin { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Tracking> Trackings { get; set; } = new List<Tracking>();
    
    // 1-1 
    public Invoice? Invoice { get; set; }

}