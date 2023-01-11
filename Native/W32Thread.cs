using PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DuDa.Windows.Native;
internal static partial class W32Thread
{
    [LibraryImport("Kernel32", EntryPoint = "SuspendThread")]
    public unsafe static partial int SuspendThread(nint hThread);

    [LibraryImport("Kernel32", EntryPoint = "TerminateThread")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public unsafe static partial bool TerminateThread(nint hThread, int exitCode);

    [LibraryImport("Kernel32", EntryPoint = "ResumeThread")]
    public unsafe static partial int ResumeThread(nint hThread);


}
