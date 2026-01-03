namespace MiniNova.BLL.DTO.Package;

public class UpdatePackageDTO
{
    public required string Description { get; set; }
    public required string Size { get; set; }
    public decimal Weight { get; set; }
    public int DestinationId { get; set; }
}