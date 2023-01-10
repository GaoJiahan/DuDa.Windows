namespace DuDa.Windows.Native;
internal struct PSS_VA_SPACE_ENTRY
{
    public nint BaseAddress;
    public nint AllocationBase;
    public int AllocationProtect;
    public nint RegionSize;
    public int State;
    public int Protect;
    public int Type;
    public int TimeDateStamp;
    public int SizeOfImage;
    public nint ImageBase;
    public int CheckSum;
    public short MappedFileNameLength;
    public nint  MappedFileName;
}
