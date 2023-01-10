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

    public unsafe static string ToString(this nint ptr, int length, int offset = 0) => new((char*)ptr, offset, length);
}
