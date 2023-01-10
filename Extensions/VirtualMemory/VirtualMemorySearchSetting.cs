using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuDa.Windows.Extensions.VirtualMemory;
public class VirtualMemorySearchSetting
{
    public VirtualMemoryProtect Protect { get; set; } = VirtualMemoryProtect.ReadWrite;

    public VirtualMemoryType Type { get; set; } = VirtualMemoryType.Private | VirtualMemoryType.Image;

    public VirtualMemoryState State { get; set; } = VirtualMemoryState.Commit;

    public VirtualMemorySearchAlignment Alignment { get; set; } = new();

    public nint MinAddress { get; set; } = 0;

    public nint MaxAddress { get; set; } = nint.MaxValue;

    public int PageSize { get; set; } = Environment.SystemPageSize;
}


