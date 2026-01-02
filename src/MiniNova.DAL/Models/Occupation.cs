namespace MiniNova.DAL.Models;

public class Occupation
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal BaseSalary { get; set; }
}