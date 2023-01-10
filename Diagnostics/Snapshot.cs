using DuDa.Windows.Extensions.UnSafe;
using DuDa.Windows.Native;
using PInvoke;
using System.Diagnostics;
using static PInvoke.Kernel32;

namespace DuDa.Windows.Diagnostics;

internal static class Snapshot
{
    public static IEnumerable<PrcoessSnapshot> QueryProcess(Func<PROCESSENTRY32, bool> predicate)
    {
        using var hSnapshot = CreateProcessSnapshot();

        if (hSnapshot.IsInvalid) yield break;

        if (new PROCESSENTRY32() { dwSize = UnSafeExtension.Size<PROCESSENTRY32>() } is var info &&
                Process32First(hSnapshot, ref info))
        {
            do
            {
                if (predicate(info)) yield return new(info.th32ProcessID, Path.GetFileNameWithoutExtension(info.ExeFile));

            } while (Process32Next(hSnapshot, ref info));

            var err = GetLastError();

            Debug.WriteLineIf(err is not Win32ErrorCode.ERROR_NO_MORE_FILES and not Win32ErrorCode.NERR_Success,
                $"<{nameof(Process32Next)}> 枚举进程非正常中断 - {err.GetLogMessage()}");
        }
        else Debug.WriteLine($"<{nameof(Process32First)}> 枚举进程失败 - {GetLastError().GetLogMessage()}");
    }

    public static SafeObjectHandle CreateProcessSnapshot()
    {
        var hSnapshot = CreateToolhelp32Snapshot(CreateToolhelp32SnapshotFlags.TH32CS_SNAPPROCESS, default);

        Debug.WriteLineIf(hSnapshot.IsInvalid, $"<{nameof(CreateToolhelp32Snapshot)}> 创建系统进程快照失败 - {GetLastError().GetLogMessage()}");

        return hSnapshot;
    }

    public static SafeObjectHandle CreateModuleSnapshot(int pid)
    {
        var hSnapshot = CreateToolhelp32Snapshot(CreateToolhelp32SnapshotFlags.TH32CS_SNAPMODULE |
            CreateToolhelp32SnapshotFlags.TH32CS_SNAPMODULE32, pid);

        Debug.WriteLineIf(hSnapshot.IsInvalid, $"<{nameof(CreateToolhelp32Snapshot)}> 创建进程(Id:{pid})模块快照失败 - {GetLastError().GetLogMessage()}");

        return hSnapshot;
    }

    public static SafeSnapshotHandle CaptureObjectSnapshot(SafeObjectHandle hProcess)
    {
        nint hSnapshot = default;

        var err = W32Snapshot.PssCaptureSnapshot(hProcess.DangerousGetHandle(),
            PSS_CAPTURE_FLAGS.PSS_CAPTURE_HANDLES |
            PSS_CAPTURE_FLAGS.PSS_CAPTURE_HANDLE_NAME_INFORMATION |
            PSS_CAPTURE_FLAGS.PSS_CAPTURE_HANDLE_BASIC_INFORMATION |
            PSS_CAPTURE_FLAGS.PSS_CAPTURE_HANDLE_TYPE_SPECIFIC_INFORMATION, default, out hSnapshot);

        Debug.WriteLineIf(err is not Win32ErrorCode.ERROR_SUCCESS, $"<{nameof(W32Snapshot.PssCaptureSnapshot)}> 抓取进程 {QueryFullProcessImageName(hProcess)} 内核对象快照失败 - {err.GetLogMessage()}");

        return new(hProcess, hSnapshot);
    }

    public static SafeSnapshotHandle CaptureVirtualMemorySnapshot(SafeObjectHandle hProcess)
    {
        nint hSnapshot = default;

        var err = W32Snapshot.PssCaptureSnapshot(hProcess.DangerousGetHandle(),
            PSS_CAPTURE_FLAGS.PSS_CAPTURE_VA_SPACE |
            PSS_CAPTURE_FLAGS.PSS_CAPTURE_VA_SPACE_SECTION_INFORMATION, default, out hSnapshot);

        Debug.WriteLineIf(err is not Win32ErrorCode.ERROR_SUCCESS, $"<{nameof(W32Snapshot.PssCaptureSnapshot)}> 抓取进程 {QueryFullProcessImageName(hProcess)} 虚拟内存信息快照失败 - {err.GetLogMessage()}");

        return new(hProcess, hSnapshot);
    }

    public static SafeSnapshotHandle CaptureProcessSnapshot(SafeObjectHandle hProcess)
    {
        nint hSnapshot = default;

        var err = W32Snapshot.PssCaptureSnapshot(hProcess.DangerousGetHandle(), default, default, out hSnapshot);

        Debug.WriteLineIf(err is not Win32ErrorCode.ERROR_SUCCESS, $"<{nameof(W32Snapshot.PssCaptureSnapshot)}> 抓取进程 {QueryFullProcessImageName(hProcess)} 虚拟内存信息快照失败 - {err.GetLogMessage()}");

        return new(hProcess, hSnapshot);
    }

    public static IEnumerable<PSS_HANDLE_ENTRY64> QueryObjectInfo64(SafeObjectHandle hProcess, PSS_OBJECT_TYPE type)
    {
        using var hSnapshot = CaptureObjectSnapshot(hProcess);

        if (hSnapshot.IsInvalid) yield break;

        var err = W32Snapshot.PssWalkMarkerCreate(default, out var hWalkMarke);

        Debug.WriteLineIf(err is not Win32ErrorCode.ERROR_SUCCESS, $"<{nameof(W32Snapshot.PssWalkMarkerCreate)}> 创建进程 {QueryFullProcessImageName(hProcess)} 步行标记失败 - {err.GetLogMessage()}");

        using SafeWalkMarkerHandle walkMarke = new(hWalkMarke);

        if (walkMarke.IsInvalid) yield break;

        var info = new PSS_HANDLE_ENTRY64();

        while (true)
        {
            err = W32Snapshot.PssWalkObjectSnapshot64(hSnapshot.DangerousGetHandle(), PSS_WALK_INFORMATION_CLASS.PSS_WALK_HANDLES, walkMarke.DangerousGetHandle(), out info, UnSafeExtension.Size<PSS_HANDLE_ENTRY64>());

            if (err is not Win32ErrorCode.ERROR_SUCCESS) break;

            if (info.ObjectType == type) yield return info;
        }

        Debug.WriteLineIf(err is not Win32ErrorCode.ERROR_NO_MORE_ITEMS, $"<{nameof(W32Snapshot.PssWalkObjectSnapshot64)}> 步行 {QueryFullProcessImageName(hProcess)} 进程快照失败 - {err.GetLogMessage()}");
    }

    public static IEnumerable<PSS_HANDLE_ENTRY32> QueryObjectInfo32(SafeObjectHandle hProcess, PSS_OBJECT_TYPE type)
    {
        using var hSnapshot = CaptureObjectSnapshot(hProcess);

        if (hSnapshot.IsInvalid) yield break;

        var err = W32Snapshot.PssWalkMarkerCreate(default, out var hWalkMarke);

        Debug.WriteLineIf(err is not Win32ErrorCode.ERROR_SUCCESS, $"<{nameof(W32Snapshot.PssWalkMarkerCreate)}> 创建进程 {QueryFullProcessImageName(hProcess)} 步行标记失败 - {err.GetLogMessage()}");

        using SafeWalkMarkerHandle walkMarke = new(hWalkMarke);

        if (walkMarke.IsInvalid) yield break;

        var info = new PSS_HANDLE_ENTRY32();

        while (true)
        {
            err = W32Snapshot.PssWalkObjectSnapshot32(hSnapshot.DangerousGetHandle(), PSS_WALK_INFORMATION_CLASS.PSS_WALK_HANDLES, walkMarke.DangerousGetHandle(), out info, UnSafeExtension.Size<PSS_HANDLE_ENTRY32>());

            if (err is not Win32ErrorCode.ERROR_SUCCESS) break;

            if (info.ObjectType == type) yield return info;
        }

        Debug.WriteLineIf(err is not Win32ErrorCode.ERROR_NO_MORE_ITEMS, $"<{nameof(W32Snapshot.PssWalkObjectSnapshot32)}> 步行 {QueryFullProcessImageName(hProcess)} 进程快照失败 - {err.GetLogMessage()}");
    }

    public static IEnumerable<PSS_VA_SPACE_ENTRY> QueryVirtualMemory(SafeObjectHandle hProcess)
    {
        using var hSnapshot = CaptureVirtualMemorySnapshot(hProcess);

        if (hSnapshot.IsInvalid) yield break;

        var err = W32Snapshot.PssWalkMarkerCreate(default, out var hWalkMarke);

        Debug.WriteLineIf(err is not Win32ErrorCode.ERROR_SUCCESS, $"<{nameof(W32Snapshot.PssWalkMarkerCreate)}> 创建进程 {QueryFullProcessImageName(hProcess)} 步行标记失败 - {err.GetLogMessage()}");

        using SafeWalkMarkerHandle walkMarke = new(hWalkMarke);

        if (walkMarke.IsInvalid) yield break;

        var info = new PSS_VA_SPACE_ENTRY();

        while (true)
        {
            err = W32Snapshot.PssWalkVirtualMemroySnapshot(hSnapshot.DangerousGetHandle(), PSS_WALK_INFORMATION_CLASS.PSS_WALK_VA_SPACE, walkMarke.DangerousGetHandle(), out info, UnSafeExtension.Size<PSS_VA_SPACE_ENTRY>());

            if (err is not Win32ErrorCode.ERROR_SUCCESS) break;

            yield return info;
        }

        Debug.WriteLineIf(err is not Win32ErrorCode.ERROR_NO_MORE_ITEMS, $"<{nameof(W32Snapshot.PssWalkVirtualMemroySnapshot)}> 步行 {QueryFullProcessImageName(hProcess)} 进程快照失败 - {err.GetLogMessage()}");
    }

    public unsafe static PSS_PROCESS_INFORMATION? QueryProcessInfo(SafeObjectHandle hProcess)
    {
        using var hSnapshot = CaptureProcessSnapshot(hProcess);

        if (hSnapshot.IsInvalid) return null!;

        var info = new PSS_PROCESS_INFORMATION();

        var err = W32Snapshot.PssQuerySnapshot(hSnapshot.DangerousGetHandle(), PSS_QUERY_INFORMATION_CLASS.PSS_QUERY_PROCESS_INFORMATION, out info, sizeof(PSS_PROCESS_INFORMATION));

        if (err is Win32ErrorCode.ERROR_SUCCESS) return info;

        Debug.WriteLine($"<{nameof(W32Snapshot.PssQuerySnapshot)}> 步行 {QueryFullProcessImageName(hProcess)} 进程快照失败 - {err.GetLogMessage()}");

        return default;
    }
}

