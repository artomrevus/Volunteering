using System.ComponentModel.DataAnnotations;

namespace Notifications.Application.Dtos.Bindings;

public class CreateBindingDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string Email { get; init; } = null!;
}