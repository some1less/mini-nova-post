using MiniNova.BLL.DTO.People;
using MiniNova.BLL.DTO.Tracking;

namespace MiniNova.BLL.DTO.Package;

public class PackageByIdDTO
{
    public int Id { get; set; }
    public required PersonResponseDTO Shipper { get; set; }
    public required PersonResponseDTO Consignee { get; set; }
    
    public required string Description { get; set; }
    public required string Size { get; set; }
    public decimal Weight { get; set; }
    
    public required string DestinationAddress { get; set; }
    
    public string Status { get; set; } = "Registered";
    public IEnumerable<TrackingResponseDTO> History { get; set; } = new List<TrackingResponseDTO>();
    
}