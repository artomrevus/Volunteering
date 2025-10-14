using System.ComponentModel.DataAnnotations;

namespace Auth.Application.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class AllowedValuesIgnoreCaseAttribute(params string[] allowedValues) : ValidationAttribute
{
    private readonly HashSet<string> _allowedValuesSet = new(allowedValues, StringComparer.OrdinalIgnoreCase);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var stringValue = value.ToString();
        if (stringValue is null)
        {
            return new ValidationResult("Value must be a string");
        }
        
        if (_allowedValuesSet.Contains(stringValue))
        {
            return ValidationResult.Success;
        }

        var formattedValues = string.Join(", ", _allowedValuesSet.Select(v => $"'{v}'"));
        var errorMessage = ErrorMessage ?? $"The value '{stringValue}' is not allowed. Allowed values are: {formattedValues}";
        
        return new ValidationResult(errorMessage);
    }
}