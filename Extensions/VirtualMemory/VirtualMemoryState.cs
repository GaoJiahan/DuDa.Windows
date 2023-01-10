namespace DuDa.Windows.Extensions.VirtualMemory;

[Flags]
public enum VirtualMemoryState : uint
{
    Commit = 0x1000,

    Reserve = 0x2000,

    Reset = 0x80000,
}

