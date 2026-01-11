using MiniNova.BLL.DTO.People;

namespace MiniNova.BLL.DTO.Package;

public class ShipmentAllDTO
{
    public int Id { get; set; }
    public required string TrackingNo { get; set; }

    public required PersonAllShipmentsDTO Sender { get; set; }
    public required PersonAllShipmentsDTO Receiver { get; set; }
    public required string Description { get; set; }
    
    public string DestinationAddress { get; set; } = string.Empty;
    public string OriginAddress { get; set; } = string.Empty;
    public string Status { get; set; }  = string.Empty;
}