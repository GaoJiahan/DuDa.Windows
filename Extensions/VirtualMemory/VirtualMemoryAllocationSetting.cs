namespace DuDa.Windows.Extensions.VirtualMemory;

public class VirtualMemoryAllocSetting
{
    required public int Size { get; init; }

    public nint Address { get; init; } = 0;

    public VirtualMemoryState Type { get; init; } = VirtualMemoryState.Commit | VirtualMemoryState.Reserve;

    public VirtualMemoryProtect Protect { get; init; } = VirtualMemoryProtect.ExecuteAndReadWrite;
}
