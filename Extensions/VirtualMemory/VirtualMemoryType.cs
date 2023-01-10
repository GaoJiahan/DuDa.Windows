﻿namespace DuDa.Windows.Extensions.VirtualMemory;

[Flags]
public enum VirtualMemoryType : uint
{
    None = 0,

    Image = 0x1000000,

    Mapped = 0x40000,

    Private = 0x20000
}

