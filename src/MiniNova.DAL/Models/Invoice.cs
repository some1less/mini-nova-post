namespace MiniNova.DAL.Models;

public class Invoice
{
    public int Id { get; set; }
    
    public int PackageId { get; set; }
    public Package Package { get; set; } = null!;
    public int PayerId { get; set; }
    public Person Payer { get; set; } = null!;
    
    public required decimal Amount { get; set; }
    public required string Status { get; set; }
    public DateTime? PaymentDate { get; set; }
}