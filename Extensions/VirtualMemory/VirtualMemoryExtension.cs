using DuDa.Windows.Native;
using DuDa.Windows.Diagnostics;
using System.Reflection.Metadata;
using static PInvoke.Kernel32;

namespace DuDa.Windows.Extensions.VirtualMemory;

public static class VirtualMemoryExtension
{
    /// <summary>
    /// 分配进程虚拟内存
    /// </summary>
    public static VirtualMemoryPointer<T> AllocMemory<T>(this Process process, VirtualMemoryAllocSetting setting) where T : unmanaged
    {
        var address = new VirtualMemoryPointer<T>(process, W32VirtualMemory.VirtualAllocEx(process.Handle, setting.Address, setting.Size, setting.Type, setting.Protect)) { isFree = true };

        System.Diagnostics.Debug.WriteLineIf(address.Address is 0, $"{nameof(W32VirtualMemory.VirtualAllocEx)} 分配进程 {QueryFullProcessImageName(process.handle)} 虚拟内存失败. - {GetLastError().GetLogMessage()}");

        return address;
    }

    /// <summary>
    /// 分配进程虚拟内存
    /// </summary>
    public static VirtualMemoryPointer<T> AllocMemory<T>(this Process process, int size) where T : unmanaged => AllocMemory<T>(process, new VirtualMemoryAllocSetting() { Size = size });

    /// <summary>
    /// 分配进程虚拟内存指针
    /// </summary>
    public static unsafe VirtualMemoryPointer<T> CreateMemoryPointer<T>(this Process process, params nint[] addresses) where T : unmanaged
    {
        switch (addresses.Length)
        {
            case 0:
                return new(process, 0);

            case 1:
                return new(process, addresses[0]);

            default:

                nint address = default;

                foreach (var addr in addresses[0..^1])
                {
                    if (!W32VirtualMemory.ReadProcessMemory(process.Handle, addr + address, &address, sizeof(nint), out _) || address is 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"{nameof(ReadProcessMemory)} 读取 {QueryFullProcessImageName(process.handle)} 进程虚拟内存失败, 内存地址:{addr + address:X}. - {GetLastError().GetLogMessage()}");

                        return new(process, 0);
                    }
                }

                return new(process, address + addresses[^1]);
        }

    }

}
