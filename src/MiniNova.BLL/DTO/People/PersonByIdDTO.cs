namespace MiniNova.BLL.DTO.People;

public class PersonByIdDTO
{
    public long Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
}