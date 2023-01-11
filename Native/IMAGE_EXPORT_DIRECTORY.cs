using PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuDa.Windows.Native;
internal struct IMAGE_EXPORT_DIRECTORY
{
    public int Characteristics;
    public int TimeDateStamp;
    public short MajorVersion;
    public short MinorVersion;
    public int Name;
    public int Base;
    public int NumberOfFunctions;
    public int NumberOfNames;
    public int AddressOfFunctions;     // RVA from base of image
    public int AddressOfNames;         // RVA from base of image
    public int AddressOfNameOrdinals;  // RVA from base of image
}
