namespace DuDa.Windows.Extensions.VirtualMemory;


[Flags]
public enum VirtualMemoryProtect : uint
{
    None = 0,

    Execute = 0x10,

    ExecuteAndRead = 0x20,

    ExecuteAndReadWrite = 0x40,

    ExecuteAndWriteCopy = 0x80,

    NoAccess = 0x01,

    ReadOnly = 0x02,

    ReadWrite = 0x04,

    WriteCopy = 0x08,

    WriteCombine = 0x400
}

