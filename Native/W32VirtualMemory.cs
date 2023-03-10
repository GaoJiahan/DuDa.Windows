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

    [LibraryImport("Kernel32", EntryPoint = "ReadProcessMemory")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public unsafe static partial bool ReadProcessMemory(nint hProcess, nint baseAddress, byte[] buffer, int size, out int numberOfBytesRead);

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

    public static unsafe T ReadProcessMemory<T>(nint hProcess, nint address) where T : unmanaged
    {
        var t = new T();

        ReadProcessMemory(hProcess, address, &t, sizeof(T), out _);

        return t;
    }

    public static unsafe T[] ReadProcessMemory<T>(nint hProcess, nint address, int length) where T : unmanaged
    {
        var datas = new T[length];

        fixed (T* ptr = datas)
            ReadProcessMemory(hProcess, address, ptr, length * sizeof(T), out _);

        return datas;
    }

    public static unsafe string ReadProcessMemory(nint hProcess, nint address)
    {
        var datas = new sbyte[256];

        fixed (sbyte* ptr = datas)
        {
            ReadProcessMemory(hProcess, address, ptr, 256, out _);

            return new(ptr);
        }
    }
}
