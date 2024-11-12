namespace ALSI.PrimitiveValidation;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Verify that a property should not be null when instantiated.
/// </summary>
public class PropertyNotNullAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext
    ) =>
        value is null
            ? new ValidationResult("This field must be initialised with a non-empty value")
            : ValidationResult.Success;
}
