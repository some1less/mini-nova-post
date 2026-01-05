using System.ComponentModel.DataAnnotations;

namespace MiniNova.BLL.DTO.People;

public class PersonDTO
{
    [Required]
    [RegularExpression(@"^[A-Z][a-z]*$", ErrorMessage = "First name must start with an uppercase letter and contain only letters.")]
    public required string FirstName { get; set; }
    [Required]
    [RegularExpression(@"^[A-Z][a-z]*$", ErrorMessage = "First name must start with an uppercase letter and contain only letters.")]
    public required string LastName { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public required string Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number.")]
    public string? Phone { get; set; }
}