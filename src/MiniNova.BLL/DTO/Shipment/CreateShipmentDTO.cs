using System.ComponentModel.DataAnnotations;

namespace MiniNova.BLL.DTO.Shipment;

public class CreateShipmentDTO
{
    
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public required string ConsigneeEmail { get; set; }
    
    [Required]
    [MinLength(5, ErrorMessage = "Description has to be longer than 5 characters.")]
    public required string Description { get; set; }

    [Required]
    public int SizeId { get; set; }
    [Required]
    [Range(0.1, 40, ErrorMessage = "Weight has to be between 0.1 and 40 kg.")]
    public decimal Weight { get; set; }
    
    [Required]
    public int DestinationId { get; set; }
    
    [Required]
    public int OriginId { get; set; }
}
