using MiniNova.BLL.DTO.People;

namespace MiniNova.BLL.DTO.Package;

public class PackageByIdDTO
{
    public int Id { get; set; }
    public PersonByIdDTO Sender { get; set; }
    public PersonByIdDTO Receiver { get; set; }
    
    public required string Description { get; set; }
    public required string Size { get; set; }
    public decimal Weight { get; set; }
    
    public required string DestinationAddress { get; set; }
    
}