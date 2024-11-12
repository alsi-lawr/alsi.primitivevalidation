using BenchmarkDotNet.Attributes;

namespace ALSI.PrimitiveValidation.Benchmarks;

using ALSI.PrimitiveValidation;
using BenchmarkDotNet.Jobs;
using System.Text.RegularExpressions;

[MemoryDiagnoser(true)]
[SimpleJob(RuntimeMoniker.Net80)]
public class Benchmarks
{
    // language=regex
    private static string Base64Regex = @"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$";
    private Regex _base64Regex = new Regex(Base64Regex);
    private static string Invalid = "my_invalid_base_64_string_that_is_long";
    private static string Valid = "bXlfaW52YWxpZF9iYXNlXzY0X3N0cmluZ190aGF0X2lzX2xvbmc=";

    [Benchmark]
    public bool IsBase64_Invalid()
    {
        return IsBase64Attribute.IsBase64Charset(Invalid);
    }

    //[Benchmark]
    //public bool IsBase64_Valid()
    //{
    //    return IsBase64Attribute.IsBase64Charset(Valid);
    //}

    //[Benchmark]
    //public bool Regex_IsBase64_Invalid()
    //{
    //    return _base64Regex.IsMatch(Invalid);
    //}

    //[Benchmark]
    //public bool Regex_IsBase64_Valid()
    //{
    //    return _base64Regex.IsMatch(Valid);
    //}

    //[Benchmark]
    //public bool Stdlib_IsBase64_Invalid()
    //{
    //    return Base64.IsValid(Invalid);
    //}

    //[Benchmark]
    //public bool Stdlib_IsBase64_Valid()
    //{
    //    return Base64.IsValid(Valid);
    //}
}
