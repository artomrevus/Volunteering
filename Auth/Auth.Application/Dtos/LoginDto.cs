using System.ComponentModel.DataAnnotations;

namespace Auth.Application.Dtos;

public class LoginDto
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; init; } = null!;
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; init; } = null!;
}