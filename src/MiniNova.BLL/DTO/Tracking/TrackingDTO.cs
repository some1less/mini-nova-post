using System.ComponentModel.DataAnnotations;

namespace MiniNova.BLL.DTO.Tracking;

public class TrackingDTO
{
    [Required]
    public int PackageId { get; set; }
    
    [Required]
    public int StatusId { get; set; }
}