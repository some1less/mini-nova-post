namespace MiniNova.DAL.Models;

public class Person
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    
    // 1-1 relations
    public Account? Account { get; set; }
    public Operator? Operator { get; set; }
    
    public ICollection<Shipment> ReceivedPackages { get; set; } = new List<Shipment>();
    public ICollection<Shipment> SentPackages { get; set; } = new List<Shipment>();
}