namespace ALSI.PrimitiveValidation.CompatibilityPatch;

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

internal static class Vector
{
    public static Vector128<byte> HighNibbleMask128 => Vector128.Create((byte)0x0F);

    public static Vector256<byte> HighNibbleMask256 => Vector256.Create((byte)0x0F);

    public static T GetElementUnsafe<TVec, T>(ref TVec vector, int index)
        where TVec : struct, IEquatable<TVec>
        where T : struct
    {
        ref T address = ref Unsafe.As<TVec, T>(ref Unsafe.AsRef(in vector));
        return Unsafe.Add(ref address, index);
    }

    public static void SetElementUnsafe<TVec, T>(in TVec vector, int index, T value)
        where TVec : struct, IEquatable<TVec>
        where T : struct
    {
        ref T address = ref Unsafe.As<TVec, T>(ref Unsafe.AsRef(in vector));
        Unsafe.Add(ref address, index) = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVec LoadUnsafe<TVec>(ref readonly short source)
        where TVec : struct, IEquatable<TVec>
    {
        ref byte address = ref Unsafe.As<short, byte>(ref Unsafe.AsRef(in source));
        return Unsafe.ReadUnaligned<TVec>(ref address);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TVec LoadUnsafe<TVec>(ref readonly short source, nuint elementOffset)
        where TVec : struct, IEquatable<TVec>
    {
        ref readonly byte address = ref Unsafe.As<short, byte>(
            ref Unsafe.Add(ref Unsafe.AsRef(in source), (nint)elementOffset)
        );
        return Unsafe.As<short, TVec>(ref Unsafe.AsRef(in source));
    }
}
