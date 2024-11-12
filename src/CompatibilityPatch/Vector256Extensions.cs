namespace ALSI.PrimitiveValidation.CompatibilityPatch;

using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

internal static class Vector256Extensions
{
    public static Vector256<byte> ShiftRight4(this Vector256<byte> value) =>
        Avx2.ShiftRightLogical(value.AsInt16(), 4).AsByte().And(Vector.HighNibbleMask256);

    public static Vector256<byte> And(this Vector256<byte> left, Vector256<byte> right) =>
        Avx2.And(left, right);

    internal static bool IsEqualTo(this Vector256<byte> left, Vector256<byte> right) =>
        Avx2.MoveMask(left.GetEqual(right)) == 0;

    internal static Vector256<byte> GetEqual(this Vector256<byte> left, Vector256<byte> right) =>
        Avx2.CompareEqual(left, right);
}
