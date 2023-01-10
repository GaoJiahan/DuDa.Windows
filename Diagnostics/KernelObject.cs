using DuDa.Windows.Native;
using PInvoke;
using System.Diagnostics;
using static PInvoke.Kernel32;

namespace DuDa.Windows.Diagnostics;
public class KernelObject
{
    required public SafeObjectHandle ProcessHandle { get; set; }

    /// <summary>
    /// 对象句柄
    /// </summary>
    public nint Handle { get; init; }


    /// <summary>
    /// 关闭进程内核句柄
    /// </summary>
    public bool CloseHandle()
    {
        if (W32Snapshot.DuplicateHandle(ProcessHandle.DangerousGetHandle(), Handle, GetCurrentProcess().DangerousGetHandle(), out _, 0, default, 1))
            return true;

        Debug.WriteLine($"<{nameof(CloseHandle)}> 关闭 {QueryFullProcessImageName(ProcessHandle)} 进程内核句柄失败, 句柄值:{Handle} - {GetLastError().GetLogMessage()}");

        return false;
    }


    /// <summary>
    /// 复制进程内核句柄
    /// </summary>
    public SafeObjectHandle CopyHandle()
    {
        if (W32Snapshot.DuplicateHandle(ProcessHandle.DangerousGetHandle(), Handle, GetCurrentProcess().DangerousGetHandle(), out var handle, 0, default, 2))
            return new(handle);

        Debug.WriteLine($"<{nameof(CopyHandle)}> 复制 {QueryFullProcessImageName(ProcessHandle)} 进程内核句柄失败, 句柄值:{Handle} - {GetLastError().GetLogMessage()}");

        return new();
    }

}
