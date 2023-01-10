namespace DuDa.Windows.Native;

internal unsafe struct PSS_PROCESS_INFORMATION
{
    public int ExitStatus;
    public nint PebBaseAddress;
    public nint AffinityMask;
    public int BasePriority;
    public int ProcessId;
    public int ParentProcessId;
    public PSS_PROCESS_FLAGS Flags;
    public long CreateTime;
    public long ExitTime;
    public long KernelTime;
    public long UserTime;
    public int PriorityClass;
    public nint PeakVirtualSize;
    public nint VirtualSize;
    public int PageFaultCount;
    public nint PeakWorkingSetSize;
    public nint WorkingSetSize;
    public nint QuotaPeakPagedPoolUsage;
    public nint QuotaPagedPoolUsage;
    public nint QuotaPeakNonPagedPoolUsage;
    public nint QuotaNonPagedPoolUsage;
    public nint PagefileUsage;
    public nint PeakPagefileUsage;
    public nint PrivateUsage;
    public int ExecuteFlags;
    public fixed char ImageFileName[260];
    public string Path
    {
        get
        {
            fixed (char* ptr = ImageFileName)
               return new(ptr);
        }

    }
}


