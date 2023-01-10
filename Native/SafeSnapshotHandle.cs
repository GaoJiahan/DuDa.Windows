using PInvoke;
using static PInvoke.Kernel32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DuDa.Windows.Native;
internal partial class SafeSnapshotHandle : SafeHandle
{
    [LibraryImport("Kernel32", EntryPoint = "PssFreeSnapshot")]
    public unsafe static partial Win32ErrorCode PssFreeSnapshot(nint hProcess, nint snapshotHandle);

    private readonly SafeObjectHandle hProcess;

    public SafeSnapshotHandle(SafeObjectHandle hProcess, nint invalidHandleValue, bool ownsHandle = true) : base(invalidHandleValue, ownsHandle)
    {
        this.hProcess = hProcess;
    }
    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            var err = PssFreeSnapshot(hProcess.DangerousGetHandle(), handle);

            if (err is not Win32ErrorCode.ERROR_SUCCESS)
            {
                Debug.WriteLine($"{nameof(PssFreeSnapshot)} 释放 {QueryFullProcessImageName(hProcess)} 进程快照失败.- {err.GetLogMessage()}");
                return false;
            }
        }

        return false;
    }
}

