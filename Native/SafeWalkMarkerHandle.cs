using PInvoke;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DuDa.Windows.Native;
internal partial class SafeWalkMarkerHandle : SafeHandle
{
    [LibraryImport("Kernel32", EntryPoint = "PssWalkMarkerFree")]
    public unsafe static partial Win32ErrorCode PssWalkMarkerFree(nint pssWalkSnapshot);

    public SafeWalkMarkerHandle(nint invalidHandleValue, bool ownsHandle = true) : base(invalidHandleValue, ownsHandle)
    {
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            var err = PssWalkMarkerFree(handle);

            if (err is not Win32ErrorCode.ERROR_SUCCESS)
            {
                Debug.WriteLine($"{nameof(PssWalkMarkerFree)} 释放行标记失败. - {err.GetLogMessage()}");

                return false;
            }
        }

        return false;
    }
}

