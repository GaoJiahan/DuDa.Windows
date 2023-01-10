namespace DuDa.Windows.Native;
internal struct MEMORY_BASIC_INFORMATION
{
    public nint BaseAddress;
    public nint AllocationBase;
    public int AllocationProtect;
    public short PartitionId;
    public nint RegionSize;
    public int State;
    public int Protect;
    public int Type;
}
