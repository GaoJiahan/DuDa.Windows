using PInvoke;
using System.Reflection.PortableExecutable;
using static PInvoke.IMAGE_OPTIONAL_HEADER;

namespace DuDa.Windows.Native;
internal unsafe struct IMAGE_OPTIONAL_HEADER64
{
    public MagicType Magic;
    public byte MajorLinkerVersion;
    public byte MinorLinkerVersion;
    public int SizeOfCode;
    public int SizeOfInitializedData;
    public int SizeOfUninitializedData;
    public int AddressOfEntryPoint;
    public int BaseOfCode;
    public long ImageBase;
    public int SectionAlignment;
    public int FileAlignment;
    public short MajorOperatingSystemVersion;
    public short MinorOperatingSystemVersion;
    public short MajorImageVersion;
    public short MinorImageVersion;
    public short MajorSubsystemVersion;
    public short MinorSubsystemVersion;
    public int Win32VersionValue;
    public int SizeOfImage;
    public int SizeOfHeaders;
    public int CheckSum;
    public SubsystemType Subsystem;
    public DllCharacteristics DllCharacteristics;
    public long SizeOfStackReserve;
    public long SizeOfStackCommit;
    public long SizeOfHeapReserve;
    public long SizeOfHeapCommit;
    public int LoaderFlags;
    public int NumberOfRvaAndSizes;
    public IMAGE_OPTIONAL_HEADER_DIRECTORIES DataDirectory;
}
