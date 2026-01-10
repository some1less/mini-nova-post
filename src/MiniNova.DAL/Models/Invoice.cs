namespace MiniNova.DAL.Models;

public class Invoice
{
    public int Id { get; set; }
    
    public int ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = null!;
    public int PayerId { get; set; }
    public Person Payer { get; set; } = null!;
    
    public required decimal Amount { get; set; }
    public required string Status { get; set; }
    public DateTime? PaymentDate { get; set; }
}