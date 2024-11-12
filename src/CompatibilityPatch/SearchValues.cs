namespace ALSI.PrimitiveValidation.CompatibilityPatch;

using System;
using System.Collections;
using System.Runtime.Intrinsics;

/// <summary>
/// Represents a search space of values.
/// </summary>
public record SearchValues : IEnumerable<char>
{
    private readonly char[] _chars;
    private readonly Vector256<byte> _bitmap;

    /// <summary>
    /// Gets the bitmap calculated from the search space.
    /// </summary>
    public Vector256<byte> Bitmap => _bitmap;

    /// <summary>
    /// Initialises a new instance of the <see cref="SearchValues"/> class.
    /// </summary>
    /// <param name="span">The space of values to form the search.</param>
    public unsafe SearchValues(ReadOnlySpan<char> span)
    {
        _chars = span.ToArray();
        Bitmapper.ComputeBitmap256(ref span, out _bitmap);
    }

    public static implicit operator SearchValues(string vals) => new(vals.AsSpan());

    /// <inheritdoc/>
    public IEnumerator<char> GetEnumerator() => (IEnumerator<char>)_chars.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _chars.GetEnumerator();
}
