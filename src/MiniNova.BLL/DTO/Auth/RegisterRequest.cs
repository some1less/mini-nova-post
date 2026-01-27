using System.ComponentModel.DataAnnotations;

namespace MiniNova.BLL.DTO.Auth;

public class RegisterRequest
{
    
    [Required]
    [MaxLength(32, ErrorMessage = "Login is too long.")]
    [MinLength(5, ErrorMessage = "Login has to be longer than 5 characters.")]
    public required string Login { get; set; }
    [Required]
    [MinLength(5, ErrorMessage = "Password has to be longer than 5 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{5,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character.")] 
    public required string Password { get; set; }

    [Required]
    [RegularExpression(@"^[A-Z][a-z]*$", ErrorMessage = "First name must start with an uppercase letter and contain only letters.")]
    public required string FirstName { get; set; }
    [Required]
    [RegularExpression(@"^[A-Z][a-z]*$", ErrorMessage = "First name must start with an uppercase letter and contain only letters.")]
    public required string LastName { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public required string Email { get; set; }

    public string? Phone { get; set; }
}