using DuDa.Windows.Native;
using System.Text;
using DuDa.Windows.Diagnostics;
using static PInvoke.Kernel32;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.InteropServices;

namespace DuDa.Windows.Extensions.VirtualMemory;


/// <summary>
/// 指向远进程内存的指针
/// </summary>
/// <remarks>
/// 注意: <br/><br/>
/// 因为 += 和 -= 符号不能重载, 所以一元 ++ 和 -- 符号都是以Step属性决定步长<br/><br/>
/// 二元 + 和 - 符号不以Step属性为步长, 统一以 1 字节来进行偏移
/// </remarks>
public class VirtualMemoryPointer : SafeHandle
{
    private readonly Process process;

    private readonly bool ownsHandle;

    public VirtualMemoryPointer(Process process, nint address, bool ownsHandle = false) : base(address, ownsHandle)
    {
        this.process = process;

        this.ownsHandle = ownsHandle;

        Address = address;
    }

    /// <summary>
    ///内存地址
    /// </summary>
    public nint Address { get; private set; }

    /// <summary>
    /// 指针自增时的步长
    /// </summary>
    public int Step { get; set; } = 1;

    /// <summary>
    /// 内存地址所处的页保护属性
    /// </summary>
    public unsafe VirtualMemoryProtect Protect
    {

        get
        {
            var result = W32VirtualMemory.VirtualQueryEx(process.Handle, Address, out var info, sizeof(MEMORY_BASIC_INFORMATION));

            System.Diagnostics.Debug.WriteLineIf(result is 0, $"{nameof(W32VirtualMemory.VirtualQueryEx)} 读取 {QueryFullProcessImageName(process.handle)} 进程虚拟内存保护属性失败, 内存地址:{Address:X}. - {GetLastError().GetLogMessage()}");

            return (VirtualMemoryProtect)info.Protect;
        }
        set
        {
            if (!W32VirtualMemory.VirtualProtectEx(process.Handle, Address, 1, value, out var old))
                System.Diagnostics.Debug.WriteLine($"{nameof(W32VirtualMemory.VirtualProtectEx)} 设置 {QueryFullProcessImageName(process.handle)} 进程虚拟内存保护属性失败, 内存地址:{Address:X}. - {GetLastError().GetLogMessage()}");
        }
    }

    public override bool IsInvalid => Address == 0;

    /// <summary>
    /// 获取内存地址处的数据
    /// </summary>
    public unsafe TYPE Get<TYPE>() where TYPE : unmanaged => Get<TYPE>(1)[0];

    /// <summary>
    /// 获取内存地址处的数据
    /// </summary>
    public unsafe TYPE[] Get<TYPE>(int length) where TYPE : unmanaged
    {
        var array = new TYPE[length];

        fixed (void* ptr = &array[0])
            if (!W32VirtualMemory.ReadProcessMemory(process.Handle, Address, ptr, sizeof(TYPE) * array.Length, out _))
                System.Diagnostics.Debug.WriteLine($"{nameof(ReadProcessMemory)} 读取 {QueryFullProcessImageName(process.handle)} 进程虚拟内存失败, 内存地址:{Address:X}. - {GetLastError().GetLogMessage()}");

        return array;
    }

    /// <summary>
    /// 设置内存地址处的数据
    /// </summary>
    public unsafe bool Set<TYPE>(params TYPE[] data) where TYPE : unmanaged
    {
        fixed (void* ptr = &data[0])
        {
            var result = W32VirtualMemory.WriteProcessMemory(process.Handle, Address, ptr, sizeof(TYPE) * data.Length, out _);

            System.Diagnostics.Debug.WriteLineIf(!result, $"{nameof(WriteProcessMemory)} 写入 {QueryFullProcessImageName(process.handle)} 进程虚拟内存失败, 内存地址:{Address:X}. - {GetLastError().GetLogMessage()}");

            return result;
        }
    }

    /// <summary>
    /// 设置内存地址处的数据
    /// </summary>
    public bool Set(string data, Encoding encoding) => Set(encoding.GetBytes(data));

    /// <summary>
    /// 获取内存地址处的 Ascii 字符串
    /// </summary>
    public unsafe string GetAscii(int length = 4096)
    {
        fixed (sbyte* ptr = &Get<sbyte>(length)[0]) return new(ptr);
    }

    /// <summary>
    /// 获取内存地址处的 Unicode 字符串
    /// </summary>
    public unsafe string GetUnicode(int length = 2048)
    {
        fixed (char* ptr = &Get<char>(length)[0]) return new(ptr);
    }

    /// <summary>
    /// 获取内存地址处的指定编码字符串
    /// </summary>
    public unsafe string GetEncoding(Encoding encoding, int length = 4096)
    {
        return encoding.GetString(Get<byte>(length).TakeWhile(x => x is not 0).ToArray());
    }

    protected override bool ReleaseHandle()
    {
        return W32VirtualMemory.VirtualFreeEx(process.Handle, handle, 0, 0x8000);
    }

    public unsafe static VirtualMemoryPointer operator ++(VirtualMemoryPointer l)
    {
        l.Address += l.Step;

        return l;
    }

    public unsafe static VirtualMemoryPointer operator --(VirtualMemoryPointer l)
    {
        l.Address -= l.Step;

        return l;
    }

    public static VirtualMemoryPointer operator +(VirtualMemoryPointer l, int r)
        => new VirtualMemoryPointer(l.process, l.Address + r, l.ownsHandle);

    public static VirtualMemoryPointer operator -(VirtualMemoryPointer l, int r)
        => l + (-r);

    public static implicit operator nint(VirtualMemoryPointer ptr) => ptr.Address;
}

