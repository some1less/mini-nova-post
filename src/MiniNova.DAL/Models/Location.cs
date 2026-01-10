namespace MiniNova.DAL.Models;

public class Location
{
    public int Id { get; set; }
    public required string Country { get; set; }
    public required string City { get; set; }
    public required string Address { get; set; }
    
    public required string Postcode { get; set; }
}