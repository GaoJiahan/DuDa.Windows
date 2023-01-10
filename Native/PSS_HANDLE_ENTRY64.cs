using System.Runtime.InteropServices;

namespace DuDa.Windows.Native;

[StructLayout(LayoutKind.Explicit, Size = 136)]
internal unsafe struct PSS_HANDLE_ENTRY64
{
    [FieldOffset(0)]
    public nint Handle;
    [FieldOffset(0x8)]
    public PSS_HANDLE_FLAGS Flags;
    [FieldOffset(0xC)]
    public PSS_OBJECT_TYPE ObjectType;
    [FieldOffset(0x10)]
    public long CaptureTime;
    [FieldOffset(0x18)]
    public int Attributes;
    [FieldOffset(0x1C)]
    public int GrantedAccess;
    [FieldOffset(0x20)]
    public int HandleCount;
    [FieldOffset(0x24)]
    public int PointerCount;
    [FieldOffset(0x28)]
    public int PagedPoolCharge;
    [FieldOffset(0x2C)]
    public int NonPagedPoolCharge;
    [FieldOffset(0x30)]
    public long CreationTime;
    [FieldOffset(0x38)]
    public short TypeNameLength;
    [FieldOffset(0x40)]
    public nint TypeName;
    [FieldOffset(0x48)]
    public short ObjectNameLength;
    [FieldOffset(0x50)]
    public nint ObjectName;

    //Process
    [FieldOffset(0x58)]
    public int ProcessExitStatus;
    [FieldOffset(0x60)]
    public nint ProcessPebBaseAddress;
    [FieldOffset(0x68)]
    public nint ProcessAffinityMask;
    [FieldOffset(0x70)]
    public int ProcessBasePriority;
    [FieldOffset(0x74)]
    public int ProcessId;
    [FieldOffset(0x78)]
    public int ProcessParentId;
    [FieldOffset(0x7C)]
    public int ProcessFlags;


    //Thread
    [FieldOffset(0x58)]
    public int ThreadExitStatus;
    [FieldOffset(0x60)]
    public nint ThreadTebBaseAddress;
    [FieldOffset(0x68)]
    public int ThreadPid;
    [FieldOffset(0x6C)]
    public int ThreadId;
    [FieldOffset(0x70)]
    public nint ThreadAffinityMask;
    [FieldOffset(0x78)]
    public int ThreadPriority;
    [FieldOffset(0x7C)]
    public int ThreadBasePriority;
    [FieldOffset(0x80)]
    public nint ThreadWin32StartAddress;

    //Mutant
    [FieldOffset(0x58)]
    public int MutantCurrentCount;
    [FieldOffset(0x5C)]
    public int MutantAbandoned;
    [FieldOffset(0x60)]
    public int MutantOwnerProcessId;
    [FieldOffset(0x64)]
    public int MutantOwnerThreadId;

    //Event
    [FieldOffset(0x58)]
    public int EventManualReset;
    [FieldOffset(0x5C)]
    public int EventSignaled;

    //Section
    [FieldOffset(0x58)]
    public nint SectionBaseAddress;
    [FieldOffset(0x60)]
    public int SectionAllocationAttributes;
    [FieldOffset(0x68)]
    public long SectionMaximumSize;

    //Semaphore
    [FieldOffset(0x58)]
    public int SemaphoreCurrentCount;
    [FieldOffset(0x5C)]
    public int SemaphoreMaximumCount;
}

