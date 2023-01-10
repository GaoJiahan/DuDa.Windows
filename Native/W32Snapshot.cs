using PInvoke;
using System.Runtime.InteropServices;

namespace DuDa.Windows.Native;

internal static partial class W32Snapshot
{

    [LibraryImport("Kernel32", EntryPoint = "PssCaptureSnapshot")]
    public unsafe static partial Win32ErrorCode PssCaptureSnapshot(nint hProcess, PSS_CAPTURE_FLAGS captureFlags, int threadContextFlags, out nint snapshotHandle);

    [LibraryImport("Kernel32", EntryPoint = "PssWalkMarkerCreate")]
    public unsafe static partial Win32ErrorCode PssWalkMarkerCreate(nint allocator, out nint walkMarkerHandle);

    [LibraryImport("Kernel32", EntryPoint = "PssWalkSnapshot")]
    public unsafe static partial Win32ErrorCode PssWalkObjectSnapshot32(nint snapshotHandle, PSS_WALK_INFORMATION_CLASS informationClass,
        nint pssWalkSnapshot, out PSS_HANDLE_ENTRY32 buffer, int bufferLength);

    [LibraryImport("Kernel32", EntryPoint = "PssWalkSnapshot")]
    public unsafe static partial Win32ErrorCode PssWalkObjectSnapshot64(nint snapshotHandle, PSS_WALK_INFORMATION_CLASS informationClass,
        nint pssWalkSnapshot, out PSS_HANDLE_ENTRY64 buffer, int bufferLength);

    [LibraryImport("Kernel32", EntryPoint = "PssWalkSnapshot")]
    public unsafe static partial Win32ErrorCode PssWalkVirtualMemroySnapshot(nint snapshotHandle, PSS_WALK_INFORMATION_CLASS informationClass,
    nint pssWalkSnapshot, out PSS_VA_SPACE_ENTRY buffer, int bufferLength);

    [LibraryImport("Kernel32", EntryPoint = "PssQuerySnapshot")]
    public unsafe static partial Win32ErrorCode PssQuerySnapshot(nint snapshotHandle, PSS_QUERY_INFORMATION_CLASS informationClass, out PSS_PROCESS_INFORMATION buffer, int bufferLength);

    [LibraryImport("Kernel32", EntryPoint = "DuplicateHandle")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal unsafe static partial bool DuplicateHandle(nint hProcessSource, nint handleSource, nint hProcessTarget, out nint handleTarget, int access, [MarshalAs(UnmanagedType.Bool)] bool inherit, int options);
}
