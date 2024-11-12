namespace ALSI.PrimitiveValidation.UnitTests;

#if NET6_0
using ALSI.PrimitiveValidation;
using ALSI.PrimitiveValidation.CompatibilityPatch;
using FluentAssertions;
using Xunit;

public class PrimitiveValidationCompatibilityTests
{
    [Fact]
    public void ContainsAnyExcept_Base64Chars_OnValidString_ShouldNotContainNonBase64()
    {
        string validString = "bXlfaW52YWxpZF9iYXNlXzY0X3N0cmluZ190aGF0X2lzX2xvbmc=";
        SearchValues searchValues =
            "+/0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz=";
        var result = searchValues
            .ContainsAnyExceptVectorised(validString.AsSpan())
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ContainsAnyExcept_Base64Chars_OnInvalidString_ShouldContainNonBase64()
    {
        string invalidString = "my_invalid_base_64_string_that_is_long";
        SearchValues searchValues =
            "+/0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz=";
        var result = searchValues
            .ContainsAnyExceptVectorised(invalidString.AsSpan())
            .Should()
            .BeTrue();
    }
}
#endif

