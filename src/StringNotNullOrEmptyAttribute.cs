namespace ALSI.PrimitiveValidation;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Verifies that a string is not null nor empty.
/// </summary>
public class StringNotNullOrEmptyAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) =>
        value is string str && string.IsNullOrEmpty(str)
            ? new ValidationResult("This field must be initialised with a non-empty value")
            : ValidationResult.Success;
}
