namespace ALSI.PrimitiveValidation.CompatibilityPatch;

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

internal static class Vector128Extensions
{
    public static Vector128<byte> ShuffleUnsafe(Vector128<byte> vector, Vector128<byte> indices) =>
        Ssse3.IsSupported ? Ssse3.Shuffle(vector, indices)
        : AdvSimd.Arm64.IsSupported ? AdvSimd.Arm64.VectorTableLookup(vector, indices)
        : Shuffle(vector, indices);

    public static Vector128<byte> Shuffle(Vector128<byte> vector, Vector128<byte> indices)
    {
        Unsafe.SkipInit(out Vector128<byte> result);

        for (int index = 0; index < Vector128<byte>.Count; index++)
        {
            byte selectedIndex = Vector.GetElementUnsafe<Vector128<byte>, byte>(ref indices, index);
            byte selectedValue = 0;

            if (selectedIndex < Vector128<byte>.Count)
            {
                selectedValue = Vector.GetElementUnsafe<Vector128<byte>, byte>(
                    ref vector,
                    selectedIndex
                );
            }

            Vector.SetElementUnsafe(result, index, selectedValue);
        }

        return result;
    }

    public static Vector128<byte> ShiftRight4(this Vector128<byte> value) =>
        Ssse3.IsSupported
            ? Sse2.And(
                Sse2.ShiftRightLogical(value.AsInt16(), 4).AsByte(),
                Vector.HighNibbleMask128
            )
        : AdvSimd.Arm64.IsSupported
            ? AdvSimd.And(
                AdvSimd.ShiftRightLogical(value.AsInt16(), 4).AsByte(),
                Vector.HighNibbleMask128
            )
        : value.GetLower().ShiftRight(4).ToVector128().WithUpper(value.GetUpper().ShiftRight(4));

    public static Vector128<byte> And(this Vector128<byte> left, Vector128<byte> right) =>
        Ssse3.IsSupported ? Sse2.And(left, right)
        : AdvSimd.Arm64.IsSupported ? AdvSimd.And(left, right).AsByte()
        : left.GetLower()
            .And(right.GetLower())
            .ToVector128()
            .WithUpper(left.GetUpper().And(right.GetUpper()));

    internal static bool IsEqualTo(this Vector128<byte> left, Vector128<byte> right) =>
        Ssse3.IsSupported ? Sse2.MoveMask(left.GetEqual(right)) == 0
        : AdvSimd.Arm64.IsSupported ? AdvSimd.Arm64.MinAcross(left.GetEqual(right)).ToScalar() == 0
        : left.GetLower().IsEqualTo(right.GetLower())
            && left.GetUpper().IsEqualTo(right.GetUpper());

    internal static Vector128<byte> GetEqual(this Vector128<byte> left, Vector128<byte> right) =>
        Ssse3.IsSupported ? Sse2.CompareEqual(left, right)
        : AdvSimd.Arm64.IsSupported ? AdvSimd.CompareEqual(left, right)
        : left.GetLower()
            .GetEqual(right.GetLower())
            .ToVector128()
            .WithUpper(left.GetUpper().GetEqual(right.GetUpper()));
}
