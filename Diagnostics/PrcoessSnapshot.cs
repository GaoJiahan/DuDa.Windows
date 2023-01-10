using DuDa.Windows.Native;
using static PInvoke.Kernel32;

namespace DuDa.Windows.Diagnostics;
public class PrcoessSnapshot
{
    public PrcoessSnapshot(int id, string name)
    {
        Id = id;

        Name = name;
    }

    public int Id { get; init; }

    public string Name { get; init; } = default!;


    /// <summary>
    /// 以指定权限打开进程
    /// </summary>
    public Process? Open(ACCESS_MASK access)
    {
        var hProcess = OpenProcess(access, default, Id);

        if (hProcess.IsInvalid)
        {
            System.Diagnostics.Debug.WriteLine($"<{nameof(OpenProcess)}> 打开进程(Id:{Id})失败 - {GetLastError().GetLogMessage()}");

            return null!;
        }

        if (Snapshot.QueryProcessInfo(hProcess) is not null and PSS_PROCESS_INFORMATION info)
        {

            return new(hProcess, System.Diagnostics.Process.GetProcessById(info.ProcessId))
            {
                Id = info.ProcessId,
                ParentId = info.ParentProcessId,
                Path = info.Path,
                IsX86 = info.Flags.HasFlag(PSS_PROCESS_FLAGS.PSS_PROCESS_FLAGS_WOW64),
                Name = System.IO.Path.GetFileNameWithoutExtension(info.Path),
            };
        }

        return null!;
    }

    /// <summary>
    /// 以指定默认的通用权限进程
    /// </summary>
    public Process? Open() => Open(ACCESS_MASK.GenericRight.GENERIC_ALL);

}

