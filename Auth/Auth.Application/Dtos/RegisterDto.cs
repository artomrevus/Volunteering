using System.ComponentModel.DataAnnotations;
using Auth.Application.Validation;
using Auth.Domain.Constants;

namespace Auth.Application.Dtos;

public class RegisterDto
{
    [Required(ErrorMessage = "Username is required")]
    [Length(3, 255, ErrorMessage = "Username length should be between 3 and 255 characters")]
    public string Username { get; init; } = null!;
    
    [Required(ErrorMessage = "Password is required")]
    [Length(8, 255, ErrorMessage = "Password length should be between 8 and 255 characters")]
    public string Password { get; init; } = null!;
    
    [Required(ErrorMessage = "Role is required")]
    [AllowedValuesIgnoreCase(UserRoles.Military, UserRoles.Volunteer)]
    public string Role { get; init; } = null!;
}