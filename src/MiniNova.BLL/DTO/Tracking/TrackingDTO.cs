using System.ComponentModel.DataAnnotations;

namespace MiniNova.BLL.DTO.Tracking;

public class TrackingDTO
{
    [Required]
    public int PackageId { get; set; }
    
    [Required]
    [MinLength(10, ErrorMessage = "Status has to be at least 10 characters")]
    public required string Status { get; set; }
}