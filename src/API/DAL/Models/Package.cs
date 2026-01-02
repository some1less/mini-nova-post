namespace API.DAL.Models;

public class Package
{
    public int Id { get; set; }
    
    public int SenderId { get; set; }
    public Person Sender { get; set; } = null!;
    public int ReceiverId { get; set; }
    public Person Receiver { get; set; } = null!;
    
    public string Description {get; set;} = null!;
    public string Size { get; set; } = null!;
    public decimal Weight { get; set; }
    
    public int DestinationId { get; set; }
    public Destination Destination { get; set; } = null!;

    public ICollection<Tracking> Trackings { get; set; } = new List<Tracking>();
    
    // 1-1 
    public Invoice? Invoice { get; set; }

}