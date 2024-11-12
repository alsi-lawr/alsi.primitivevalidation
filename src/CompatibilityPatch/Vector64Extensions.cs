namespace ALSI.PrimitiveValidation.CompatibilityPatch;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

internal static class Vector64Extensions
{
    internal static Vector64<byte> ShiftRight(this Vector64<byte> value, int shiftCount)
    {
        Unsafe.SkipInit(out Vector64<byte> result);

        for (int index = 0; index < Vector64<byte>.Count; index++)
        {
            byte element = (byte)(object)Vector.GetElementUnsafe<Vector64<byte>, byte>(ref value, index);

            element = (byte)(element >>> (shiftCount & 7));
            Vector.SetElementUnsafe(in result, index, element);
        }

        return result;
    }

    internal static Vector64<byte> And(this Vector64<byte> left, Vector64<byte> right)
    {
        Unsafe.SkipInit(out Vector64<byte> result);
        unsafe
        {
            var new00 = left._00() & right._00();
            set00(ref result, new00);
        }

        return result;
    }

    internal static ulong _00(this ref Vector64<byte> vector)
    {
        // Use reflection to get the private field
        ulong inner00 = GetFieldRef<ulong>(ref vector, "_00");
        return inner00;
    }

    internal static void set00(ref Vector64<byte> value, ulong new00)
    {
        ref ulong inner00 = ref GetFieldRef<ulong>(ref value, "_00");
        inner00 = new00;
    }

    internal static unsafe ref TField GetFieldRef<TField>(
        ref Vector64<byte> value,
        string fieldName
    )
    {
        IntPtr fieldOffset = Marshal.OffsetOf<Vector64<byte>>(fieldName);
        fixed (void* p = &value)
        {
            IntPtr objPtr = *(IntPtr*)&p;

            byte* fieldAddress = (byte*)objPtr + fieldOffset.ToInt64();
            return ref Unsafe.AsRef<TField>(fieldAddress);
        }
    }

    internal static bool IsEqualTo(this Vector64<byte> left, Vector64<byte> right)
    {
        for (int index = 0; index < Vector64<byte>.Count; index++)
        {
            if (!CompareAtIndex(ref left, ref right, index))
            {
                return false;
            }
        }

        return true;
    }

    internal static bool CompareAtIndex(
        ref Vector64<byte> left,
        ref Vector64<byte> right,
        int index
    ) =>
        (byte)(object)Vector.GetElementUnsafe<Vector64<byte>, byte>(ref left, index)
        == (byte)(object)Vector.GetElementUnsafe<Vector64<byte>, byte>(ref right, index);

    internal static Vector64<byte> GetEqual(this Vector64<byte> left, Vector64<byte> right)
    {
        Unsafe.SkipInit(out Vector64<byte> result);

        for (int index = 0; index < Vector64<byte>.Count; index++)
        {
            byte value = CompareAtIndex(ref left, ref right, index)
                ? (byte)(object)byte.MaxValue
                : default!;

            Vector.SetElementUnsafe(in result, index, value);
        }

        return result;
    }
}
