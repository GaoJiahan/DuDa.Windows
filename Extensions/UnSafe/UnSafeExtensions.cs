using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuDa.Windows.Extensions.UnSafe;
public static class UnSafeExtension
{
    public unsafe static int Size<T>(this T _) where T : unmanaged => sizeof(T);

    public unsafe static int Size<T>() where T : unmanaged => sizeof(T);

    public unsafe static ref T To<T>(this byte[] values, int offset) where T : unmanaged
    {
        fixed (byte* ptr = &values[offset])
        {
            return ref *(T*)ptr;
        }
    }

    public unsafe static ref T To<T>(this byte[] values, uint offset) where T : unmanaged
        => ref To<T>(values, (int)offset);

    public unsafe static string ToString(this nint ptr, int length, int offset = 0) => new((char*)ptr, offset, length);

    public unsafe static string ToAscii(byte* ptr) => new((sbyte*)ptr);

    public unsafe static string ToAscii(this byte[] values, int offset)
    {
        fixed (byte* ptr = &values[offset])
        {
            return new((sbyte*)ptr);
        }
    }
}
