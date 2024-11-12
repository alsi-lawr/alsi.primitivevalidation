#pragma warning disable SA1500
namespace ALSI.PrimitiveValidation.CompatibilityPatch;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

/// <summary>
/// NET6.0 compatibility patch for exposing SIMD-powered extension utilities.
/// </summary>
public static class IndexOfAnyAsciiSearcher
{
    internal static bool IsVectorizationSupported =>
        Ssse3.IsSupported || AdvSimd.Arm64.IsSupported || Avx2.IsSupported;

    /// <summary>
    /// Searches for any value other than the specified values.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="values">The set of values to exclude from the search.</param>
    /// <returns>true if any value other than those in values is present in the span. If all of the values are in values, returns false.</returns>
    public static bool ContainsAnyExceptVectorised(
        this SearchValues span,
        ReadOnlySpan<char> values
    ) =>
        TryContainsAnyExcept(
            ref Unsafe.As<char, char>(ref MemoryMarshal.GetReference(values)),
            values.Length,
            ref span
        );

    /// <summary>
    /// Searches for any value other than the specified values.
    /// </summary>
    /// <param name="span">The span to search.</param>
    /// <param name="values">The set of values to exclude from the search.</param>
    /// <returns>true if any value other than those in values is present in the span. If all of the values are in values, returns false.</returns>
    public static bool ContainsAnyExcept(this SearchValues span, ReadOnlySpan<char> values)
    {
        foreach (char c in span)
        {
            if (!values.Contains(c))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryContainsAny(
        ref char searchSpace,
        int searchSpaceLength,
        ref SearchValues asciiValues
    ) =>
        TryContainsAny<DontNegate>(
            ref Unsafe.As<char, short>(ref searchSpace),
            searchSpaceLength,
            asciiValues
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryContainsAnyExcept(
        ref char searchSpace,
        int searchSpaceLength,
        ref SearchValues asciiValues
    ) =>
        TryContainsAny<Negate>(
            ref Unsafe.As<char, short>(ref searchSpace),
            searchSpaceLength,
            asciiValues
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe bool TryContainsAny<TNegator>(
        ref short searchSpace,
        int searchSpaceLength,
        SearchValues asciiValues
    )
        where TNegator : struct, INegator
    {
        if (Avx2.IsSupported)
        {
            return ContainsAnyVectorized<TNegator, Default>(
                ref searchSpace,
                searchSpaceLength,
                ref asciiValues
            );
        }

        var lower = asciiValues.Bitmap.GetLower();
        return ContainsAnyVectorized<TNegator, Default>(
            ref searchSpace,
            searchSpaceLength,
            ref lower
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ContainsAnyVectorized<TNegator, TOptimizations>(
        ref short searchSpace,
        int searchSpaceLength,
        ref Vector128<byte> bitmapRef
    )
        where TNegator : struct, INegator
        where TOptimizations : struct, IOptimizations
    {
        ref short currentSearchSpace = ref searchSpace;

        Vector128<byte> bitmap = bitmapRef;

        ref short twoVectorsAwayFromEnd = ref Unsafe.Add(
            ref searchSpace,
            searchSpaceLength - 2 * Vector128<short>.Count
        );

        do
        {
            Vector128<short> source0 = Vector.LoadUnsafe<Vector128<short>>(ref currentSearchSpace);
            Vector128<short> source1 = Vector.LoadUnsafe<Vector128<short>>(
                ref currentSearchSpace,
                (nuint)Vector128<short>.Count
            );

            bool result = ContainsAnyLookup<TNegator, TOptimizations>(source0, source1, bitmap);
            if (result)
            {
                return true;
            }

            currentSearchSpace = ref Unsafe.Add(ref currentSearchSpace, 2 * Vector128<short>.Count);
        } while (Unsafe.IsAddressLessThan(ref currentSearchSpace, ref twoVectorsAwayFromEnd));
        {
            ref short oneVectorAwayFromEnd = ref Unsafe.Add(
                ref searchSpace,
                searchSpaceLength - Vector128<short>.Count
            );

            ref short firstVector = ref Unsafe.IsAddressGreaterThan(
                ref currentSearchSpace,
                ref oneVectorAwayFromEnd
            )
                ? ref oneVectorAwayFromEnd
                : ref currentSearchSpace;

            Vector128<short> source0 = Vector.LoadUnsafe<Vector128<short>>(ref firstVector);
            Vector128<short> source1 = Vector.LoadUnsafe<Vector128<short>>(
                ref oneVectorAwayFromEnd
            );

            bool result = ContainsAnyLookup<TNegator, TOptimizations>(source0, source1, bitmap);
            if (result)
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ContainsAnyVectorized<TNegator, TOptimizations>(
        ref short searchSpace,
        int searchSpaceLength,
        ref SearchValues searchVals
    )
        where TNegator : struct, INegator
        where TOptimizations : struct, IOptimizations
    {
        ref short currentSearchSpace = ref searchSpace;

        Vector256<byte> bitmap = searchVals.Bitmap;

        ref short twoVectorsAwayFromEnd = ref Unsafe.Add(
            ref searchSpace,
            searchSpaceLength - 2 * Vector256<short>.Count
        );

        do
        {
            Vector256<short> source0 = Vector.LoadUnsafe<Vector256<short>>(ref currentSearchSpace);
            Vector256<short> source1 = Vector.LoadUnsafe<Vector256<short>>(
                ref currentSearchSpace,
                (nuint)Vector256<short>.Count
            );

            bool result = ContainsAnyLookup<TNegator, TOptimizations>(source0, source1, bitmap);
            if (result)
            {
                return true;
            }

            currentSearchSpace = ref Unsafe.Add(ref currentSearchSpace, 2 * Vector256<short>.Count);
        } while (Unsafe.IsAddressLessThan(ref currentSearchSpace, ref twoVectorsAwayFromEnd));
        {
            ref short oneVectorAwayFromEnd = ref Unsafe.Add(
                ref searchSpace,
                searchSpaceLength - Vector256<short>.Count
            );

            ref short firstVector = ref Unsafe.IsAddressGreaterThan(
                ref currentSearchSpace,
                ref oneVectorAwayFromEnd
            )
                ? ref oneVectorAwayFromEnd
                : ref currentSearchSpace;

            Vector256<short> source0 = Vector.LoadUnsafe<Vector256<short>>(ref firstVector);
            Vector256<short> source1 = Vector.LoadUnsafe<Vector256<short>>(
                ref oneVectorAwayFromEnd
            );

            bool result = ContainsAnyLookup<TNegator, TOptimizations>(source0, source1, bitmap);
            if (result)
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ContainsAnyLookup<TNegator, TOptimizations>(
        Vector128<short> source0,
        Vector128<short> source1,
        Vector128<byte> bitmapLookup
    )
        where TNegator : struct, INegator
        where TOptimizations : struct, IOptimizations
    {
        Vector128<byte> source = TOptimizations.PackSources(source0.AsUInt16(), source1.AsUInt16());

        Vector128<byte> result = ContainsAnyLookupCore(source, bitmapLookup);

        return TNegator.NegateIfNeeded(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ContainsAnyLookup<TNegator, TOptimizations>(
        Vector256<short> source0,
        Vector256<short> source1,
        Vector256<byte> bitmapLookup
    )
        where TNegator : struct, INegator
        where TOptimizations : struct, IOptimizations
    {
        Vector256<byte> source = TOptimizations.PackSources(source0.AsUInt16(), source1.AsUInt16());

        Vector256<byte> result = ContainsAnyLookupCore(source, bitmapLookup);

        return TNegator.NegateIfNeeded(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<byte> ContainsAnyLookupCore(
        Vector128<byte> source,
        Vector128<byte> bitmapLookup
    )
    {
        Vector128<byte> lowNibbles = Ssse3.IsSupported
            ? source
            : source.And(Vector.HighNibbleMask128);

        Vector128<byte> highNibbles = source.ShiftRight4();

        Vector128<byte> bitMask = Vector128Extensions.ShuffleUnsafe(bitmapLookup, lowNibbles);

        Vector128<byte> bitPositions = Vector128Extensions.ShuffleUnsafe(
            Vector128.Create(0x8040201008040201, 0).AsByte(),
            highNibbles
        );

        Vector128<byte> result = bitMask.And(bitPositions);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector256<byte> ContainsAnyLookupCore(
        Vector256<byte> source,
        Vector256<byte> bitmapLookup
    )
    {
        Vector256<byte> highNibbles = source.ShiftRight4();
        Vector256<byte> bitMask = Avx2.Shuffle(bitmapLookup, source);
        Vector256<byte> bitPositions = Avx2.Shuffle(
            Vector256.Create(0x8040201008040201).AsByte(),
            highNibbles
        );
        Vector256<byte> result = bitMask.And(bitPositions);
        return result;
    }
}

internal interface INegator
{
    public static abstract bool NegateIfNeeded(bool result);

    public static abstract bool NegateIfNeeded(Vector128<byte> result);

    public static abstract bool NegateIfNeeded(Vector256<byte> result);
}

internal readonly struct DontNegate : INegator
{
    public static bool NegateIfNeeded(bool result) => result;

    public static bool NegateIfNeeded(Vector128<byte> result) => true;

    public static bool NegateIfNeeded(Vector256<byte> result) => true;
}

internal readonly struct Negate : INegator
{
    public static bool NegateIfNeeded(bool result) => !result;

    public static bool NegateIfNeeded(Vector128<byte> result) =>
        !result.IsEqualTo(Vector128<byte>.Zero);

    public static bool NegateIfNeeded(Vector256<byte> result) =>
        !result.IsEqualTo(Vector256<byte>.Zero);
}

internal interface IOptimizations
{
    public static abstract Vector128<byte> PackSources(
        Vector128<ushort> lower,
        Vector128<ushort> upper
    );

    public static abstract Vector256<byte> PackSources(
        Vector256<ushort> lower,
        Vector256<ushort> upper
    );
}

internal readonly struct Default : IOptimizations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<byte> PackSources(Vector128<ushort> lower, Vector128<ushort> upper) =>
        Sse3.IsSupported ? Sse2.PackUnsignedSaturate(lower.AsInt16(), upper.AsInt16())
        : AdvSimd.IsSupported
            ? AdvSimd.ExtractNarrowingSaturateUpper(
                AdvSimd.ExtractNarrowingSaturateLower(lower),
                upper
            )
        : throw new PlatformNotSupportedException();

    public static Vector256<byte> PackSources(Vector256<ushort> lower, Vector256<ushort> upper) =>
        Avx2.PackUnsignedSaturate(lower.AsInt16(), upper.AsInt16());
}
#pragma warning restore SA1500
