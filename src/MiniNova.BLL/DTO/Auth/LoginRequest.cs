using System.ComponentModel.DataAnnotations;

namespace MiniNova.BLL.DTO.Auth;

public class LoginRequest
{
    [Required]
    [MaxLength(32, ErrorMessage = "Login is too long.")]
    [MinLength(5, ErrorMessage = "Login has to be longer than 5 characters.")]
    public required string Login { get; set; }
    [Required]
    [MinLength(5, ErrorMessage = "Password has to be longer than 5 characters.")]
    public required string Password { get; set; }
}