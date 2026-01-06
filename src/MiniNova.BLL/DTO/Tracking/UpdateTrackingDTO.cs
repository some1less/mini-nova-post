using System.ComponentModel.DataAnnotations;

namespace MiniNova.BLL.DTO.Tracking;

public class UpdateTrackingDTO
{
    [Required]
    [MinLength(7, ErrorMessage = "Status has to be at least 7 characters")]
    public required string Status { get; set; }
}