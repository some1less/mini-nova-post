namespace API.DAL.Models;

public class Operator
{
    public int Id { get; set; }
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    
    public int OccupationId { get; set; }
    public Occupation  Occupation { get; set; } = null!;
    
    public int PersonId { get; set; }
    public Person  Person { get; set; } = null!;
    
    
}