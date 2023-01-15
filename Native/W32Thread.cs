using PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PInvoke.Kernel32;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    [LibraryImport("Kernel32", EntryPoint = "CreateRemoteThread")]
    public unsafe static partial nint CreateRemoteThread(nint hProcess, nint threadAttributes, nint stackSize, nint startAddress, nint parameter, int creationFlags, out int tid);
}
