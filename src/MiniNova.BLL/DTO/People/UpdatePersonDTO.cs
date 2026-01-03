namespace MiniNova.BLL.DTO.People;

public class UpdatePersonDTO
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string?  Phone { get; set; }
}