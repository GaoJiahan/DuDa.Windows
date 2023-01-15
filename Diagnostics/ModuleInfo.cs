using DuDa.Windows.Extensions.PE;
using DuDa.Windows.Extensions.UnSafe;
using DuDa.Windows.Native;
using static DuDa.Windows.Native.W32VirtualMemory;

namespace DuDa.Windows.Diagnostics;


public class ModuleInfo
{
    /// <summary>
    /// 模块句柄; 也是模块的加载地址
    /// </summary>
    required public nint Handle { get; init; }

    /// <summary>
    /// 模块大小
    /// </summary>
    required public int Size { get; init; }

    /// <summary>
    /// 模块名称
    /// </summary>
    required public string Name { get; init; }

    /// <summary>
    /// 模块文件路径
    /// </summary>
    required public string Path { get; init; }


    /// <summary>
    /// 所属进程句柄,仅限内部使用
    /// </summary>
    internal nint hProcess { get; init; }


    /// <summary>
    /// 所有导出函数信息
    /// </summary>
    public IEnumerable<ExportFuntion> ExportFunctions
    {
        get
        {
            var pe = new PE(ReadProcessMemory<byte>(hProcess, Handle, 0x1000));

            if (pe.DataDirectory.ExportTable.VirtualAddress is 0) yield break;

            var tExport = ReadProcessMemory<IMAGE_EXPORT_DIRECTORY>(hProcess, Handle + (nint)pe.DataDirectory.ExportTable.VirtualAddress);

            var offsets = ReadProcessMemory<int>(hProcess, Handle + tExport.AddressOfFunctions, tExport.NumberOfFunctions);

            var ordinals = ReadProcessMemory<short>(hProcess, Handle + tExport.AddressOfNameOrdinals, tExport.NumberOfFunctions);

            var names = ReadProcessMemory<int>(hProcess, Handle + tExport.AddressOfNames, tExport.NumberOfNames).Select(pName => ReadProcessMemory(hProcess, Handle + pName)).ToArray();

            for (var i = 0; i < offsets.Length; i++)
                for (var j = 0; j < ordinals.Length; j++)
                    if (ordinals[j] == i)
                        yield return new()
                        {
                            Address = Handle + offsets[i],
                            Name = j <= names.Length ? names[j] : (j + tExport.Base).ToString()
                        };
        }
    }

    /// <summary>
    /// 所有导入函数信息
    /// </summary>
    public IEnumerable<KeyValuePair<string, IEnumerable<ImportFuntion>>> ImportFuntions
    {
        get
        {
            var pe = new PE(ReadProcessMemory<byte>(hProcess, Handle, 0x1000));

            nint begin = Handle + (nint)pe.DataDirectory.ImportTable.VirtualAddress;

            while (true)
            {
                IMAGE_IMPORT_DESCRIPTOR tImport = ReadProcessMemory<IMAGE_IMPORT_DESCRIPTOR>(hProcess, begin);

                if (tImport.OriginalFirstThunk is 0) break;

                string moduleName = ReadProcessMemory(hProcess, Handle + tImport.Name);

                List<ImportFuntion> list = new();

                nint intBegin = Handle + tImport.OriginalFirstThunk;

                nint iatBegin = Handle + tImport.FirstThunk;

                if (pe.IsPE32)
                {
                    while (ReadProcessMemory<uint>(hProcess, intBegin) is uint intValue and not 0)
                    {
                        list.Add(new()
                        {
                            Name = (intValue & 0x80000000) is not 0 ?
                                (intValue ^ 0x80000000).ToString() : ReadProcessMemory(hProcess, Handle + (nint)intValue + 2),
                            IatAddress = iatBegin,
                        });

                        intBegin += 4;

                        iatBegin += 4;
                    }
                }
                else
                {

                    while (ReadProcessMemory<ulong>(hProcess, intBegin) is ulong intValue and not 0)
                    {
                        list.Add(new()
                        {
                            Name = (intValue & 0x8000000000000000) is not 0 ?
                                (intValue ^ 0x8000000000000000).ToString() : ReadProcessMemory(hProcess, Handle + (nint)intValue + 2),
                            IatAddress = iatBegin,
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
}

