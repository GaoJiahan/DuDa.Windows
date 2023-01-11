using DuDa.Windows.Extensions.UnSafe;
using DuDa.Windows.Native;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using static PInvoke.IMAGE_FILE_HEADER;
using static PInvoke.IMAGE_OPTIONAL_HEADER;

namespace DuDa.Windows.Extensions.PE;

public static class PEExtension
{
    public unsafe static string GetName(ref this IMAGE_SECTION_HEADER section)
    {
        fixed (byte* ptr = section.Name)
            return new string((sbyte*)ptr);
    }

    public unsafe static bool SetName(ref this IMAGE_SECTION_HEADER section, string name)
    {
        if (name.Length > 7) return false;

        var bytes = Encoding.ASCII.GetBytes(name);

        fixed (byte* ptr = section.Name)
        {
            for (var i = 0; i < name.Length; i++ )
            {
                *(ptr + i) = bytes[i];
            }

            *(ptr + bytes.Length) = 0;
        }

        return true;
    }
}

public unsafe class PE
{

    private byte[] bytes;


    private int ntOffset;

    public PE(byte[] bytes)
    {
        this.bytes = bytes;

        ntOffset = BitConverter.ToInt32(bytes, 0x3C);
    }

    public void Save(string path) => File.WriteAllBytes(path, bytes);

    internal ref IMAGE_NT_HEADERS32 IMAGE_NT_HEADERS32 => ref bytes.To<IMAGE_NT_HEADERS32>(ntOffset);

    internal ref IMAGE_NT_HEADERS64 IMAGE_NT_HEADERS64 => ref bytes.To<IMAGE_NT_HEADERS64>(ntOffset);

    public Span<IMAGE_SECTION_HEADER> SectonHeaders
    {
        get
        {
            fixed (void* ptr = &bytes[ntOffset + sizeof(IMAGE_FILE_HEADER) + SizeOfOptionalHeader + 4])
                return new Span<IMAGE_SECTION_HEADER>(ptr, NumberOfSections);
        }
    }
    
    public MachineType Machine => IMAGE_NT_HEADERS32.FileHeader.Machine;

    public CharacteristicsType Characteristics => IMAGE_NT_HEADERS32.FileHeader.Characteristics;

    public MagicType Magic => IMAGE_NT_HEADERS32.OptionalHeader.Magic;

    public SubsystemType Subsystem => IMAGE_NT_HEADERS32.OptionalHeader.Subsystem;

    public DllCharacteristics DllCharacteristics => IMAGE_NT_HEADERS32.OptionalHeader.DllCharacteristics;

    public ref ushort NumberOfSections => ref IMAGE_NT_HEADERS32.FileHeader.NumberOfSections;

    public short SizeOfOptionalHeader => (short)IMAGE_NT_HEADERS32.FileHeader.SizeOfOptionalHeader;

    public bool IsPE32 => SizeOfOptionalHeader is 0xE0;

    public ref int EntryPoint => ref IMAGE_NT_HEADERS32.OptionalHeader.AddressOfEntryPoint;

    public long ImageBase => IsPE32 ? IMAGE_NT_HEADERS32.OptionalHeader.ImageBase : IMAGE_NT_HEADERS64.OptionalHeader.ImageBase;

    public int SectionAlignment => IMAGE_NT_HEADERS32.OptionalHeader.SectionAlignment;

    public int FileAlignment => IMAGE_NT_HEADERS32.OptionalHeader.FileAlignment;

    public int CheckSum => IMAGE_NT_HEADERS32.OptionalHeader.CheckSum;

    public ref int SizeOfImage => ref IMAGE_NT_HEADERS32.OptionalHeader.SizeOfImage;

    public int SizeOfHeaders => IMAGE_NT_HEADERS32.OptionalHeader.SizeOfHeaders;

    public IMAGE_OPTIONAL_HEADER_DIRECTORIES DataDirectory => IsPE32 ? IMAGE_NT_HEADERS32.OptionalHeader.DataDirectory : IMAGE_NT_HEADERS64.OptionalHeader.DataDirectory;

    
}
