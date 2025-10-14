using System.ComponentModel.DataAnnotations;

namespace Notifications.Application.Dtos.Bindings;

public class UpdateBindingDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string Email { get; init; } = null!;
}