using DuDa.Windows.Extensions.UnSafe;
using DuDa.Windows.Extensions.VirtualMemory;
using DuDa.Windows.Native;
using PInvoke;
using System.Diagnostics;
using static PInvoke.Kernel32;

namespace DuDa.Windows.Diagnostics;

public class Process : IDisposable
{
    #region 私有字段

    internal SafeObjectHandle handle;

    private System.Diagnostics.Process p;

    #endregion

    #region 构造方法

    internal Process(SafeObjectHandle handle, System.Diagnostics.Process p)
    {
        this.handle = handle;
        this.p = p;
    }

    #endregion

    #region 静态方法

    /// <summary>
    /// 查询系统进程
    /// </summary>
    public static IEnumerable<PrcoessSnapshot> GetProcesses() => Snapshot.QueryProcess(_ => true);


    /// <summary>
    /// 查询系统名称为 <paramref name="name"/> 的进程
    /// </summary>
    public static IEnumerable<PrcoessSnapshot> GetProcessByName(string name) => Snapshot.QueryProcess(x => System.IO.Path.GetFileNameWithoutExtension(x.ExeFile).ToLower() == name.ToLower());


    /// <summary>
    /// 查询系统 Id 为 <paramref name="pid"/> 的进程
    /// </summary>
    public static PrcoessSnapshot GetProcessById(int pid) => Snapshot.QueryProcess(x => x.th32ProcessID == pid).Single();

    #endregion

    #region 成员属性

    /// <summary>
    /// 进程句柄
    /// </summary>
    public nint Handle => handle.DangerousGetHandle();

    /// <summary>
    /// 进程Id
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// 进程的父进程Id
    /// </summary>
    public int ParentId { get; init; }

    /// <summary>
    /// 是否为 64 位操作系统下的 32 位进程
    /// </summary>
    required public bool IsX86 { get; init; }

    /// <summary>
    /// 进程是否被冻结
    /// </summary>
    /// <remarks>
    /// 例如调试器被附加并中断到进程中，或者存储进程被生存期管理服务挂起
    /// </remarks>
    public bool IsFrozen
        => Snapshot.QueryProcessInfo(handle)?.Flags.HasFlag(PSS_PROCESS_FLAGS.PSS_PROCESS_FLAGS_FROZEN) ?? false;

    /// <summary>
    /// 进程是否受保护
    /// </summary>
    public bool IsProtected
        => Snapshot.QueryProcessInfo(handle)?.Flags.HasFlag(PSS_PROCESS_FLAGS.PSS_PROCESS_FLAGS_PROTECTED) ?? false;

    /// <summary>
    /// 进程文件路径
    /// </summary>
    public string Path { get; init; } = default!;

    /// <summary>
    /// 进程名称
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// 指示关联进程是否已终止的值
    /// </summary>
    /// <remarks>
    /// 如果进程已终止，则为 true；否则为 false
    /// </remarks>
    public bool HasExited => p.HasExited;

    /// <summary>
    /// 进程终止的退出码
    /// </summary>
    /// <remarks>
    /// 进程尚未退出或句柄无效会引发 <see cref="InvalidOperationException"/> 异常
    /// </remarks>
    public int ExitCode => p.ExitCode;

    /// <summary>
    /// 启动的时间
    /// </summary>
    public DateTime StartTime => p.StartTime;

    /// <summary>
    /// 进程退出的时间
    /// </summary>
    public DateTime ExitTime => p.ExitTime;

    /// <summary>
    /// 设置一个方法, 在进程退出时发生
    /// </summary>
    public EventHandler Exited
    {
        set
        {
            p.EnableRaisingEvents = true;
            p.Exited += value;
        }
    }

    /// <summary>
    /// 进程的基本优先级
    /// </summary>
    public int BasePriority => p.BasePriority;

    /// <summary>
    /// 获取或设置关联进程的总体优先级类别
    /// </summary>
    public System.Diagnostics.ProcessPriorityClass PriorityClass
    {
        get { return p.PriorityClass; }
        set { p.PriorityClass = value; }
    }

    /// <summary>
    /// 进程打开的句柄数
    /// </summary>
    public int HandleCount => p.HandleCount;

    /// <summary>
    /// 进程的所有模块
    /// </summary>
    public IEnumerable<ModuleInfo> Modules
    {
        get
        {
            using var hSnapshot = Snapshot.CreateModuleSnapshot(Id);

            if (hSnapshot.IsInvalid) yield break;

            if (new MODULEENTRY32() { dwSize = UnSafeExtension.Size<MODULEENTRY32>() } is var info && Module32First(hSnapshot, ref info))
            {
                do
                {
                    yield return new()
                    {
                        Size = (int)info.modBaseSize,
                        Handle = info.hModule_IntPtr,
                        Name = info.Module,
                        Path = info.ExePath,
                        hProcess = Handle
                    };

                } while (Module32Next(hSnapshot, ref info));

                var err = GetLastError();

                Debug.WriteLineIf(err is not Win32ErrorCode.ERROR_NO_MORE_FILES and not Win32ErrorCode.NERR_Success,
                    $"<{nameof(Process32Next)}> 枚举 {Name}(Id:{Id}) 进程模块非正常中断 - {err.GetLogMessage()}");
            }
            else
                Debug.WriteLine($"<{nameof(Process32Next)}> 枚举 {Name}(Id:{Id}) 进程模块失败 - {GetLastError().GetLogMessage()}");
        }
    }

    /// <summary>
    /// 进程的所有事件内核对象
    /// </summary>
    public IEnumerable<EventInfo> Events
    {
        get
        {
            if (Environment.Is64BitProcess)
            {
                foreach (var info in Snapshot.QueryObjectInfo64(handle, PSS_OBJECT_TYPE.PSS_OBJECT_TYPE_EVENT))
                {
                    yield return new()
                    {
                        ProcessHandle = handle,
                        Handle = info.Handle,
                        Name = info.ObjectNameLength is 0 ? default : info.ObjectName.ToString(info.ObjectNameLength / 2),
                        ManualReset = info.EventManualReset is not 0,
                        Signaled = info.EventSignaled is not 0
                    };
                }
            }
            else
            {
                foreach (var info in Snapshot.QueryObjectInfo32(handle, PSS_OBJECT_TYPE.PSS_OBJECT_TYPE_EVENT))
                {
                    yield return new()
                    {
                        ProcessHandle = handle,
                        Handle = info.Handle,
                        Name = info.ObjectNameLength is 0 ? default : info.ObjectName.ToString(info.ObjectNameLength / 2),
                        ManualReset = info.EventManualReset is not 0,
                        Signaled = info.EventSignaled is not 0
                    };
                }
            }

        }
    }

    /// <summary>
    /// 进程的所有互斥体内核对象
    /// </summary>
    public IEnumerable<MutantInfo> Mutants
    {
        get
        {
            if (Environment.Is64BitProcess)
            {
                foreach (var info in Snapshot.QueryObjectInfo64(handle, PSS_OBJECT_TYPE.PSS_OBJECT_TYPE_MUTANT))
                {
                    yield return new()
                    {
                        ProcessHandle = handle,
                        Handle = info.Handle,
                        Name = info.ObjectNameLength is 0 ? default : info.ObjectName.ToString(info.ObjectNameLength / 2),
                        Abandoned = info.MutantAbandoned is not 0,
                        CurrentCount = info.MutantCurrentCount
                    };
                }
            }
            else
            {
                foreach (var info in Snapshot.QueryObjectInfo32(handle, PSS_OBJECT_TYPE.PSS_OBJECT_TYPE_MUTANT))
                {
                    yield return new()
                    {
                        ProcessHandle = handle,
                        Handle = info.Handle,
                        Name = info.ObjectNameLength is 0 ? default : info.ObjectName.ToString(info.ObjectNameLength / 2),
                        Abandoned = info.MutantAbandoned is not 0,
                        CurrentCount = info.MutantCurrentCount
                    };
                }
            }

        }
    }

    /// <summary>
    /// 进程的所有虚拟内存区域信息
    /// </summary>
    public IEnumerable<VirtualMemoryRegion> MemoryRegions
    {
        get
        {
            foreach (var info in Snapshot.QueryVirtualMemory(handle))
            {
                yield return new()
                {
                    Type = (VirtualMemoryType)info.Type,
                    BaseAddress = info.BaseAddress,
                    AllocationBase = info.AllocationBase,
                    AllocationProtect = (VirtualMemoryProtect)info.AllocationProtect,
                    Protect = (VirtualMemoryProtect)info.Protect,
                    Size = info.RegionSize,
                    State = (VirtualMemoryState)info.State
                };
            }
        }
    }

    #endregion

    #region 实例方法

    public void Dispose()
    {
        if (!handle.IsInvalid) handle.Dispose();

        p?.Dispose();
    }


    #endregion
}

