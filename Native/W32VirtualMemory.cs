using DuDa.Windows.Extensions.VirtualMemory;
using System.Runtime.InteropServices;

namespace DuDa.Windows.Native;
internal static partial class W32VirtualMemory
{
    [LibraryImport("Kernel32", EntryPoint = "WriteProcessMemory")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public unsafe static partial bool WriteProcessMemory(nint hProcess, nint baseAddress, void* buffer, int size, out int numberOfBytesWritten);

    [LibraryImport("Kernel32", EntryPoint = "ReadProcessMemory")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public unsafe static partial bool ReadProcessMemory(nint hProcess, nint baseAddress, void* buffer, int size, out int numberOfBytesRead);

    [LibraryImport("Kernel32", EntryPoint = "VirtualProtectEx")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal unsafe static partial bool VirtualProtectEx(nint hProcess, nint address, int size, VirtualMemoryProtect newProtect, out VirtualMemoryProtect oldProtect);

    [LibraryImport("Kernel32", EntryPoint = "VirtualQueryEx")]
    public unsafe static partial int VirtualQueryEx(nint hProcess, nint address, out MEMORY_BASIC_INFORMATION buffer, int length);

    [LibraryImport("Kernel32", EntryPoint = "VirtualAllocEx")]
    public static partial nint VirtualAllocEx(nint hProcess, nint address, int size, VirtualMemoryState allocationType, VirtualMemoryProtect protect);

    [LibraryImport("Kernel32", EntryPoint = "VirtualFreeEx")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool VirtualFreeEx(nint hProcess, nint address, int size, int freeType);
}
