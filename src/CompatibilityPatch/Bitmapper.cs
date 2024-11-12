namespace ALSI.PrimitiveValidation.CompatibilityPatch;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

internal static class Bitmapper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe void ComputeBitmap128(
        ref ReadOnlySpan<char> values,
        out Vector128<byte> bitmap
    )
    {
        bitmap = default;
        fixed (Vector128<byte>* local = &bitmap)
        {
            ComputeBitmap(values, (byte*)local);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe void ComputeBitmap256(
        ref ReadOnlySpan<char> values,
        out Vector256<byte> bitmap
    )
    {
        ComputeBitmap128(ref values, out Vector128<byte> local);
        bitmap = Vector256.Create(local, local);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe void ComputeBitmap(ReadOnlySpan<char> values, byte* bitmap)
    {
        byte* bitmapLocal = bitmap;

        foreach (char c in values)
        {
            if (c > 127)
            {
                continue;
            }

            int highNibble = c >> 4;
            int lowNibble = c & 0xF;

            bitmapLocal[(uint)lowNibble] |= (byte)(1 << highNibble);
        }
    }
}
