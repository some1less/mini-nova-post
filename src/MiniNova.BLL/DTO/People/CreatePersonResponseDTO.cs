namespace MiniNova.BLL.DTO.People;

public class CreatePersonResponseDTO
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
}