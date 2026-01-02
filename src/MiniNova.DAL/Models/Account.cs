namespace MiniNova.DAL.Models;

public class Account
{
    public int Id { get; set; }
    public required string Login { get; set; }
    public required string Password { get; set; }
    
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;
}