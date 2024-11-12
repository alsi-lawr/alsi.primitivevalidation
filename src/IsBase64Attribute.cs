namespace ALSI.PrimitiveValidation;

using System;
using System.ComponentModel.DataAnnotations;
using ALSI.PrimitiveValidation.CompatibilityPatch;
#if NET8_0_OR_GREATER
using System.Buffers.Text;
#endif

/// <summary>
/// Ensures that a string is base 64 encoded.
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false
)]
public class IsBase64Attribute : ValidationAttribute
{
    private static readonly SearchValues Base64Charset =
        "+/0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    internal static bool IsBase64Charset(ReadOnlySpan<char> str) =>
        IndexOfAnyAsciiSearcher.IsVectorizationSupported
            ? !Base64Charset.ContainsAnyExceptVectorised(
                str[..(str.Length - 3 + str[^3..].IndexOf('='))]
            )
            : !Base64Charset.ContainsAnyExcept(str[..(str.Length - 3 + str[^3..].IndexOf('='))]);

#pragma warning disable SA1202 // Elements should be ordered by access
#if NET6_0
    /// <inheritdoc/>
    public override bool IsValid(object? value) =>
        value is string str
        && !string.IsNullOrEmpty(str)
        && str.Length % 4 == 0
        && IsBase64Charset(str);
#elif NET8_0_OR_GREATER
    /// <inheritdoc/>
    public override bool IsValid(object? value) => value is string str && Base64.IsValid(str);
#endif

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext
    ) =>
        this.IsValid(value)
            ? ValidationResult.Success
            : new ValidationResult("This field must be valid Base64.");
#pragma warning restore SA1202 // Elements should be ordered by access
}
