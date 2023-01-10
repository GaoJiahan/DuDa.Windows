using System.Runtime.InteropServices;

namespace DuDa.Windows.Native;

[StructLayout(LayoutKind.Explicit, Size = 104)]
internal unsafe struct PSS_HANDLE_ENTRY32
{
    [FieldOffset(0)]
    public nint Handle;
    [FieldOffset(0x4)]
    public PSS_HANDLE_FLAGS Flags;
    [FieldOffset(0x8)]
    public PSS_OBJECT_TYPE ObjectType;
    [FieldOffset(0xC)]
    public long CaptureTime;
    [FieldOffset(0x14)]
    public int Attributes;
    [FieldOffset(0x18)]
    public int GrantedAccess;
    [FieldOffset(0x1C)]
    public int HandleCount;
    [FieldOffset(0x20)]
    public int PointerCount;
    [FieldOffset(0x24)]
    public int PagedPoolCharge;
    [FieldOffset(0x28)]
    public int NonPagedPoolCharge;
    [FieldOffset(0x2C)]
    public long CreationTime;
    [FieldOffset(0x34)]
    public short TypeNameLength;
    [FieldOffset(0x38)]
    public nint TypeName;
    [FieldOffset(0x3C)]
    public short ObjectNameLength;
    [FieldOffset(0x40)]
    public nint ObjectName;

    //Process
    [FieldOffset(0x48)]
    public int ProcessExitStatus;
    [FieldOffset(0x4C)]
    public nint ProcessPebBaseAddress;
    [FieldOffset(0x50)]
    public nint ProcessAffinityMask;
    [FieldOffset(0x54)]
    public int ProcessBasePriority;
    [FieldOffset(0x58)]
    public int ProcessId;
    [FieldOffset(0x5C)]
    public int ProcessParentId;
    [FieldOffset(0x60)]
    public int ProcessFlags;

    //Thread
    [FieldOffset(0x48)]
    public int ThreadExitStatus;
    [FieldOffset(0x4C)]
    public nint ThreadTebBaseAddress;
    [FieldOffset(0x50)]
    public int ThreadPid;
    [FieldOffset(0x54)]
    public int ThreadId;
    [FieldOffset(0x58)]
    public nint ThreadAffinityMask;
    [FieldOffset(0x5C)]
    public int ThreadPriority;
    [FieldOffset(0x60)]
    public int ThreadBasePriority;
    [FieldOffset(0x64)]
    public nint ThreadWin32StartAddress;

    //Mutant
    [FieldOffset(0x48)]
    public int MutantCurrentCount;
    [FieldOffset(0x4C)]
    public int MutantAbandoned;
    [FieldOffset(0x50)]
    public int MutantOwnerProcessId;
    [FieldOffset(0x54)]
    public int MutantOwnerThreadId;

    //Event
    [FieldOffset(0x48)]
    public int EventManualReset;
    [FieldOffset(0x4C)]
    public int EventSignaled;

    //Section
    [FieldOffset(0x48)]
    public nint SectionBaseAddress;
    [FieldOffset(0x4C)]
    public int SectionAllocationAttributes;
    [FieldOffset(0x50)]
    public long SectionMaximumSize;

    //Semaphore
    [FieldOffset(0x48)]
    public int SemaphoreCurrentCount;
    [FieldOffset(0x4C)]
    public int SemaphoreMaximumCount;
}
