namespace ALSI.PrimitiveValidation.UnitTests;

using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using Xunit;

public class IsBase64Tests
{
    [Fact]
    public void IsBase64_WhenBase64_ShouldInstantiate()
    {
        var base64Test = new Base64TestDTO("bXlfdmFsaWRfYmFzZV82NF9zdHJpbmdfdGhhdF9pc19sb24=");

        List<ValidationResult> validationErrors = [];
        var isbase64TestValid = Validator.TryValidateObject(
            base64Test,
            new ValidationContext(base64Test),
            validationErrors,
            validateAllProperties: true
        );

        isbase64TestValid.Should().BeTrue();
    }

    [Fact]
    public void IsBase64_WhenBase64Chars_WrongLength_ShouldThrow()
    {
        var base64Test = new Base64TestDTO("bXlfdmFsaWRfYmFzZV82NF9zdHJpbmdfdGhhdF9pc19sb2");

        List<ValidationResult> validationErrors = [];
        var isbase64TestValid = Validator.TryValidateObject(
            base64Test,
            new ValidationContext(base64Test),
            validationErrors,
            validateAllProperties: true
        );

        isbase64TestValid.Should().BeFalse();
    }

    [Fact]
    public void IsBase64_WhenBase64Chars_WrongChars_ShouldThrow()
    {
        var base64Test = new Base64TestDTO("bXlfdmFsaWRfYmFzZV82NF9zdHJpbmdfdGhhdF9pc19sb2!=");
        List<ValidationResult> validationErrors = [];
        var isbase64TestValid = Validator.TryValidateObject(
            base64Test,
            new ValidationContext(base64Test),
            validationErrors,
            validateAllProperties: true
        );

        isbase64TestValid.Should().BeFalse();
    }

    public static string CharToHex(char c)
    {
        return $"0x{((int)c).ToString("X2")}";
    }
}

public record Base64TestDTO([property: IsBase64][property: Required] string Value);

