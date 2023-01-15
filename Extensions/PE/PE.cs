using DuDa.Windows.Extensions.UnSafe;
using DuDa.Windows.Native;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Intrinsics.X86;
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
            for (var i = 0; i < name.Length; i++)
            {
                *(ptr + i) = bytes[i];
            }

            *(ptr + bytes.Length) = 0;
        }

        return true;
    }
}

public record ImportFunctionInfo
{
    public string Name { get; set; } = default!;

    public int IatFoa { get; set; }

    public int IatRva { get; set; }

    public int Hint { get; set; }
}
public unsafe class PE
{
    private byte[] bytes;

    private int ntOffset;

    public static PE Load(string path) => new(File.ReadAllBytes(path));

    public PE(byte[] bytes)
    {
        this.bytes = bytes;

        ntOffset = BitConverter.ToInt32(bytes, 0x3C);
    }

    public void Save(string path) => File.WriteAllBytes(path, bytes);

    public uint ToFOA(uint rva) => SectonHeaders.ToArray().
        Where(section => section.VirtualAddress <= rva && (section.VirtualAddress + section.PhysicalAddressOrVirtualSize) >= rva).
        Select(section => rva - section.VirtualAddress + section.PointerToRawData).Single();


    public int ToFOA(int rva) => (int)ToFOA((uint)rva);

    internal ref IMAGE_NT_HEADERS32 IMAGE_NT_HEADERS32 => ref bytes.To<IMAGE_NT_HEADERS32>(ntOffset);

    internal ref IMAGE_NT_HEADERS64 IMAGE_NT_HEADERS64 => ref bytes.To<IMAGE_NT_HEADERS64>(ntOffset);

    internal ref IMAGE_EXPORT_DIRECTORY IMAGE_EXPORT_DIRECTORY => ref bytes.To<IMAGE_EXPORT_DIRECTORY>((int)ToFOA(DataDirectory.ExportTable.VirtualAddress));

    internal ref IMAGE_IMPORT_DESCRIPTOR IMAGE_IMPORT_DESCRIPTOR => ref bytes.To<IMAGE_IMPORT_DESCRIPTOR>((int)ToFOA(DataDirectory.ImportTable.VirtualAddress));

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

    internal IMAGE_OPTIONAL_HEADER_DIRECTORIES DataDirectory => IsPE32 ? IMAGE_NT_HEADERS32.OptionalHeader.DataDirectory : IMAGE_NT_HEADERS64.OptionalHeader.DataDirectory;

    public IEnumerable<(int Offset, string Name)> ExprotFunctions
    {
        get
        {
            for (var i = 0; i < IMAGE_EXPORT_DIRECTORY.NumberOfFunctions; i++)
            {
                var offset = BitConverter.ToInt32(bytes, ToFOA(IMAGE_EXPORT_DIRECTORY.AddressOfFunctions) + i * 4);

                for (var j = 0; j < IMAGE_EXPORT_DIRECTORY.NumberOfFunctions; j++)
                {
                    if (BitConverter.ToInt16(bytes, ToFOA(IMAGE_EXPORT_DIRECTORY.AddressOfNameOrdinals) + j * 2) == i)
                    {
                        if (j <= IMAGE_EXPORT_DIRECTORY.NumberOfNames)
                        {
                            var pName = ToFOA(BitConverter.ToInt32(bytes, ToFOA(IMAGE_EXPORT_DIRECTORY.AddressOfNames) + j * 4));

                            yield return (offset, bytes.ToAscii(pName));
                        }
                        else yield return (offset, (j + IMAGE_EXPORT_DIRECTORY.Base).ToString());
                    }
                }
            }
        }
    }

    public IEnumerable<KeyValuePair<string, IEnumerable<ImportFunctionInfo>>> ImportModules
    {
        get
        {
            int begin = (int)ToFOA(DataDirectory.ImportTable.VirtualAddress);

            while (true)
            {
                IMAGE_IMPORT_DESCRIPTOR tImport = bytes.To<IMAGE_IMPORT_DESCRIPTOR>(begin);

                if (tImport.OriginalFirstThunk is 0) break;

                string moduleName = bytes.ToAscii(ToFOA(tImport.Name));

                List<ImportFunctionInfo> list = new();

                int intBegin = ToFOA(tImport.OriginalFirstThunk);

                int iatBegin = tImport.FirstThunk;

                if (IsPE32)
                {
                    while (bytes.To<uint>(intBegin) is uint intValue and not 0)
                    {
                        var isById = (intValue & 0x80000000) is not 0;

                        list.Add(new()
                        {
                            Name = isById ? (intValue ^ 0x80000000).ToString() : bytes.ToAscii(ToFOA((int)intValue) + 2),
                            IatFoa = ToFOA(iatBegin),
                            IatRva = iatBegin,
                            Hint = isById ? default : bytes.To<short>(ToFOA((int)intValue))
                        });

                        intBegin += 4;

                        iatBegin += 4;
                    }
                }
                else
                {
                    while (bytes.To<ulong>(intBegin) is ulong intValue and not 0)
                    {
                        var isById = (intValue & 0x8000000000000000) is not 0;

                        list.Add(new()
                        {
                            Name = isById ? (intValue ^ 0x8000000000000000).ToString() : bytes.ToAscii(ToFOA((int)intValue) + 2),
                            IatFoa = ToFOA(iatBegin),
                            IatRva = iatBegin,
                            Hint = isById ? default : bytes.To<short>(ToFOA((int)intValue))
                        });

                        intBegin += 8;

                        iatBegin += 8;
                    }
                }

                yield return new(moduleName, list);

                begin += tImport.Size();
            }
        }
    }

    public void RestoreRelocationTable(nint imageBase)
    {
        int begin = (int)ToFOA(DataDirectory.BaseRelocationTable.VirtualAddress);

        while (true)
        {
            IMAGE_BASE_RELOCATION tReloaction = bytes.To<IMAGE_BASE_RELOCATION>(begin);

            if (tReloaction.VirtualAddress is 0) break;

            for (var i = begin + 8; i < begin + tReloaction.SizeOfBlock; i += 2)
            {
                var value = BitConverter.ToInt16(bytes, i);

                if ((value & 0b0011_0000_0000_0000) is not 0)
                {
                    var foa = (value ^ 0b0011_0000_0000_0000) + tReloaction.VirtualAddress;

                    var old = bytes.To<int>(foa);

                    bytes.To<int>(foa) = old - (int)ImageBase + (int)imageBase;

                }
                else if ((value & 0b1010_0000_0000_0000) is not 0)
                {
                    var foa = (value ^ 0b1010_0000_0000_0000) + tReloaction.VirtualAddress;

                    var old = bytes.To<long>(foa);

                    bytes.To<long>(foa) = old - ImageBase + imageBase;
                }
            }

            begin += tReloaction.SizeOfBlock;
        }
    }
}
