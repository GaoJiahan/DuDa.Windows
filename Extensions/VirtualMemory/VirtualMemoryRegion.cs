namespace DuDa.Windows.Extensions.VirtualMemory;


public class VirtualMemoryRegion
{
    public nint BaseAddress { get; set; }

    public nint AllocationBase { get; set; }

    public VirtualMemoryProtect AllocationProtect { get; set; }

    public nint Size { get; set; }

    public VirtualMemoryState State { get; set; }

    public VirtualMemoryProtect Protect { get; set; }

    public VirtualMemoryType Type { get; set; }
}
