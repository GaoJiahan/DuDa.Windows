using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuDa.Windows.Native;
public struct IMAGE_IMPORT_DESCRIPTOR
{
    public int OriginalFirstThunk;         // RVA to original unbound IAT (PIMAGE_THUNK_DATA)
    public int TimeDateStamp;                  // 0 if not bound,
    public int ForwarderChain;                 // -1 if no forwarders
    public int Name;
    public int FirstThunk;                     // RVA to IAT (if bound this IAT has actual addresses)
} 
