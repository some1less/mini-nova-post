using MiniNova.BLL.DTO.People;

namespace MiniNova.BLL.DTO.Package;

public class PackageAllDTO
{
    public int Id { get; set; }
    public required PersonAllPackagesDTO Sender { get; set; }
    public required PersonAllPackagesDTO Receiver { get; set; }
    public required string Description { get; set; }
    
    public string DestinationAddress { get; set; } = string.Empty;
    public string Status { get; set; } = "Registered";
}