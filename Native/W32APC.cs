using PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DuDa.Windows.Native;
internal static partial class W32APC
{
    [LibraryImport("Kernel32", EntryPoint = "QueueUserAPC")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public unsafe static partial bool QueueUserAPC(nint apcProc, nint hThread, nint param);

}
