using System.ComponentModel.DataAnnotations;

namespace MiniNova.BLL.DTO.Tracking;

public class UpdateTrackingDTO
{
    [Required]
    public int StatusId { get; set; }
}