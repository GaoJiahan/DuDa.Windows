using DuDa.Windows.Diagnostics;
using DuDa.Windows.Native;
using Microsoft.VisualBasic;
using PInvoke;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.PortableExecutable;

namespace DuDa.Windows.Extensions.VirtualMemory;

internal static class HelpExtension
{
    /// <summary>
    /// 返回精确浮点数范围的工具方法
    /// </summary>
    internal static (double min, double max) Range(this double target)
    {
        var sTarget = target.ToString();

        if (!sTarget.Contains('.')) return (Math.CopySign(Math.Abs(target) - 1, target), Math.CopySign(Math.Abs(target) + 1, target));
        else
        {
            var pLen = target.ToString().Split(".")[1].Length;

            var pow = Math.Pow(10, pLen);

            return (Math.CopySign(((Math.Abs(target) * pow - 1) / pow), target), Math.CopySign(((Math.Abs(target) * pow + 1) / pow), target));
        }
    }

    /// <summary>
    /// 返回精确浮点数范围的工具方法
    /// </summary>
    internal static (double min, double max) Range(this float target)
    {
        var sTarget = target.ToString();

        if (!sTarget.Contains('.')) return (Math.CopySign(Math.Abs(target) - 1, target), Math.CopySign(Math.Abs(target) + 1, target));
        else
        {
            var pLen = target.ToString().Split(".")[1].Length;

            var pow = Math.Pow(10, pLen);

            return (Math.CopySign(((Math.Abs(target) * pow - 1) / pow), target), Math.CopySign(((Math.Abs(target) * pow + 1) / pow), target));
        }
    }

    /// <summary>
    /// 浮点数范围比较的工具算法
    /// </summary>
    internal static bool Cmp(this double data, double min, double max)
    {
        return double.IsNegative(min) ? data <= min && data >= max : data >= min && data <= max;
    }

    /// <summary>
    /// 浮点数范围比较的工具算法
    /// </summary>
    internal static bool Cmp(this float data, double min, double max)
    {
        return double.IsNegative(min) ? data <= min && data >= max : data >= min && data <= max;
    }

    internal static IEnumerable<nint> PagingHelp(this IEnumerable<VirtualMemoryRegion> regions, VirtualMemorySearchSetting setting)
    {
        foreach (var region in regions)
        {
            for (var i = region.BaseAddress; i < region.BaseAddress + region.Size; i += setting.PageSize)
            {
                if (i >= setting.MinAddress && i + setting.PageSize <= setting.MaxAddress)
                    yield return i;
            }
        }
    }

}

public static class VirtualMemorySearch
{
    #region 工具方法

    /// <summary>
    /// 对指定进程内存分块的工具方法(以页为单位,返回块区首地址)
    /// </summary>
    internal static IEnumerable<nint> Paging(this Process process, VirtualMemorySearchSetting setting)
    {
        return process.MemoryRegions.AsParallel().Where(regions =>
               (setting.Protect & regions.Protect) is not 0 &&
               (setting.Type & regions.Type) is not 0 &&
               (setting.State & regions.State) is not 0).PagingHelp(setting).ToArray();
    }

    /// <summary>
    /// 搜索内存页字节数组(使用了Sunday算法)
    /// </summary>
    internal unsafe static void SerachPage(this Process process, nint address, byte[] sub, int[] go,
        VirtualMemorySearchSetting setting, ConcurrentBag<VirtualMemoryPointer<nint>> list)
    {
        var buffer = stackalloc byte[setting.PageSize];

        W32VirtualMemory.ReadProcessMemory(process.Handle, address, buffer, setting.PageSize, out _);

        var master = new Span<byte>(buffer, setting.PageSize);

        int i = 0, j = 0;

        while (i < master.Length - sub.Length)
        {
            if (master[i] == sub[j])
            {
                i++;
                j++;
            }
            else
            {
                i += go[master[i + sub.Length]] - j;
                j = 0;
            }

            if (j >= sub.Length)
            {
                list.Add(new VirtualMemoryPointer<nint>(process, i - j + address));
                j = 0;
            }
        }
    }

    /// <summary>
    /// 内存页搜索工具方法
    /// </summary>
    internal unsafe static void SerachPage(this Process process,nint address,
        VirtualMemorySearchSetting setting, ConcurrentBag<VirtualMemoryPointer<nint>> list, Func<byte, bool> predicate)
    {
        var buffer = stackalloc byte[setting.PageSize];

        W32VirtualMemory.ReadProcessMemory(process.Handle, address, buffer, setting.PageSize, out _);

        for (var i = 0; i < setting.PageSize; i += setting.Alignment.Byte)
            if (*(buffer + i) is var value && predicate(value))
                list.Add(new VirtualMemoryPointer<nint>(process, i + address));
    }

    /// <summary>
    /// 内存页搜索工具方法
    /// </summary>
    internal unsafe static void SerachPage(this Process process, nint address,
        VirtualMemorySearchSetting setting, ConcurrentBag<VirtualMemoryPointer<nint>> list, Func<short, bool> predicate)
    {
        var buffer = stackalloc byte[setting.PageSize];

        W32VirtualMemory.ReadProcessMemory(process.Handle, address, buffer, setting.PageSize, out _);

        for (var i = 0; i < setting.PageSize; i += setting.Alignment.Short)
            if (*(short*)(buffer + i) is var value && predicate(value))
                list.Add(new VirtualMemoryPointer<nint>(process, i + address));
    }

    /// <summary>
    /// 内存页搜索工具方法
    /// </summary>
    internal unsafe static void SerachPage(this Process process, nint address,
        VirtualMemorySearchSetting setting, ConcurrentBag<VirtualMemoryPointer<nint>> list, Func<int, bool> predicate)
    {
        var buffer = stackalloc byte[setting.PageSize];

        W32VirtualMemory.ReadProcessMemory(process.Handle, address, buffer, setting.PageSize, out _);

        for (var i = 0; i < setting.PageSize; i += setting.Alignment.Int)
            if (*(int*)(buffer + i) is var value && predicate(value))
                list.Add(new VirtualMemoryPointer<nint>(process, i + address));
    }

    /// <summary>
    /// 内存页搜索工具方法
    /// </summary>
    internal unsafe static void SerachPage(this Process process, nint address,
        VirtualMemorySearchSetting setting, ConcurrentBag<VirtualMemoryPointer<nint>> list, Func<long, bool> predicate)
    {
        var buffer = stackalloc byte[setting.PageSize];

        W32VirtualMemory.ReadProcessMemory(process.Handle, address, buffer, setting.PageSize, out _);

        for (var i = 0; i < setting.PageSize; i += setting.Alignment.Long)
            if (*(long*)(buffer + i) is var value && predicate(value))
                list.Add(new VirtualMemoryPointer<nint>(process, i + address));
    }

    /// <summary>
    /// 内存页搜索工具方法
    /// </summary>
    internal unsafe static void SerachPage(this Process process, nint address,
        VirtualMemorySearchSetting setting, ConcurrentDictionary<VirtualMemoryPointer<nint>, float> dict, Func<float, bool> predicate)
    {
        var buffer = stackalloc byte[setting.PageSize];

        W32VirtualMemory.ReadProcessMemory(process.Handle, address, buffer, setting.PageSize, out _);

        for (var i = 0; i < setting.PageSize; i += setting.Alignment.Float)
            if (*(float*)(buffer + i) is var value && predicate(value))
                dict[new VirtualMemoryPointer<nint>(process, i + address)] = value;
    }

    /// <summary>
    /// 内存页搜索工具方法
    /// </summary>
    internal unsafe static void SerachPage(this Process process, nint address,
        VirtualMemorySearchSetting setting, ConcurrentDictionary<VirtualMemoryPointer<nint>, double> dict, Func<double, bool> predicate)
    {
        var buffer = stackalloc byte[setting.PageSize];

        W32VirtualMemory.ReadProcessMemory(process.Handle, address, buffer, setting.PageSize, out _);

        for (var i = 0; i < setting.PageSize; i += setting.Alignment.Double)
            if (*(double*)(buffer + i) is var value && predicate(value))
                dict[new VirtualMemoryPointer<nint>(process, i + address)] = value;
    }

    #endregion


    #region 静态方法

    /// <summary>
    /// 搜索虚拟内存字节数组
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer<nint>> SearchMemory(this Process process, byte[] target,
        Func<VirtualMemorySearchSetting, VirtualMemorySearchSetting> action)
    {
        var setting =  action(new());

        var list = new ConcurrentBag<VirtualMemoryPointer<nint>>();

        var go = new int[256];

        Array.Fill(go, target.Length + 1);

        for (var v = 0; v < target.Length; v++) go[target[v]] = target.Length - v;

        Parallel.ForEach(process.Paging(setting).ToList(), address => process.SerachPage(address, target, go, setting, list));

        return list;
    }

    /// <summary>
    /// 搜索虚拟内存字节数组
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer<nint>> SearchMemory(this Process process, byte[] target)       
        => SearchMemory(process, target, setting => setting);

    /// <summary>
    /// 精确搜索虚拟内存数据(浮点数返回一个字典, 内存地址 + 初始的精确值)
    /// </summary>
    /// <remarks>
    /// 浮点数提供了一个精确值的默认搜索逻辑(模仿 CE 搜索), 如果不符合你的需求, 可以使用谓词的搜索方法<br/><br/>
    /// 如果没有小数, 范围在目标数加 1 到减 1 的范围内<br/><br/>
    /// 如果有小数, 则是小数点末位数 加 1 到减 1 的范围内
    /// </remarks>
    public static IEnumerable<KeyValuePair<VirtualMemoryPointer<nint>, float>> SearchMemory(this Process process, float target,
        Func<VirtualMemorySearchSetting, VirtualMemorySearchSetting> action)     
    {
        var setting = action(new());

        var (min, max) = target.Range();

        var dict = new ConcurrentDictionary<VirtualMemoryPointer<nint>, float>();

        Parallel.ForEach(process.Paging(setting).ToList(), address => process.SerachPage(address, setting, dict, (float value) => value.Cmp(min, max)));

        return dict;
    }

    /// <summary>
    /// 精确搜索虚拟内存数据(浮点数返回一个字典, 内存地址 + 初始的精确值)
    /// </summary>
    /// <remarks>
    /// 浮点数提供了一个精确值的默认搜索逻辑(模仿 CE 搜索), 如果不符合你的需求, 可以使用谓词的搜索方法<br/><br/>
    /// 如果没有小数, 范围在目标数加 1 到减 1 的范围内<br/><br/>
    /// 如果有小数, 则是小数点末位数 加 1 到减 1 的范围内
    /// </remarks>
    public static IEnumerable<KeyValuePair<VirtualMemoryPointer<nint>, double>> SearchMemory(this Process process, double target,
        Func<VirtualMemorySearchSetting, VirtualMemorySearchSetting> action)
    {
        var setting = action(new());

        var (min, max) = target.Range();

        var dict = new ConcurrentDictionary<VirtualMemoryPointer<nint>, double>();

        Parallel.ForEach(process.Paging(setting).ToList(), address => process.SerachPage(address, setting, dict, (double value) => value.Cmp(min, max)));

        return dict;
    }

    /// <summary>
    /// 精确搜索虚拟内存数据(浮点数返回一个字典, 内存地址 + 初始的精确值)
    /// </summary>
    /// <remarks>
    /// 浮点数提供了一个精确值的默认搜索逻辑(模仿 CE 搜索), 如果不符合你的需求, 可以使用谓词的搜索方法<br/><br/>
    /// 如果没有小数, 范围在目标数加 1 到减 1 的范围内<br/><br/>
    /// 如果有小数, 则是小数点末位数 加 1 到减 1 的范围内
    /// </remarks>
    public static IEnumerable<KeyValuePair<VirtualMemoryPointer<nint>, float>> SearchMemory(this Process process, float target) 
        => SearchMemory(process, target, setting => setting);

    /// <summary>
    /// 精确搜索虚拟内存数据(浮点数返回一个字典, 内存地址 + 初始的精确值)
    /// </summary>
    /// <remarks>
    /// 浮点数提供了一个精确值的默认搜索逻辑(模仿 CE 搜索), 如果不符合你的需求, 可以使用谓词的搜索方法<br/><br/>
    /// 如果没有小数, 范围在目标数加 1 到减 1 的范围内<br/><br/>
    /// 如果有小数, 则是小数点末位数 加 1 到减 1 的范围内
    /// </remarks>
    public static IEnumerable<KeyValuePair<VirtualMemoryPointer<nint>, double>> SearchMemory(this Process process, double target)
        => SearchMemory(process, target, setting => setting);

    /// <summary>
    /// 搜索虚拟内存中满足指定谓词的数据
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer<nint>> SearchMemory(this Process process, Func<byte, bool> predicate,
        Func<VirtualMemorySearchSetting, VirtualMemorySearchSetting> action) 
    {
        var setting = action(new());

        ConcurrentBag<VirtualMemoryPointer<nint>> dict = new();

        Parallel.ForEach(process.Paging(setting).ToList(), address => process.SerachPage(address, setting, dict, (byte value) => predicate(value)));

        return dict;
    }

    /// <summary>
    /// 搜索虚拟内存中满足指定谓词的数据
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer<nint>> SearchMemory(this Process process, Func<short, bool> predicate,
        Func<VirtualMemorySearchSetting, VirtualMemorySearchSetting> action)
    {
        var setting = action(new());

        ConcurrentBag<VirtualMemoryPointer<nint>> dict = new();

        Parallel.ForEach(process.Paging(setting).ToList(), address => process.SerachPage(address, setting, dict, (short value) => predicate(value)));

        return dict;
    }

    /// <summary>
    /// 搜索虚拟内存中满足指定谓词的数据
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer<nint>> SearchMemory(this Process process, Func<int, bool> predicate,
        Func<VirtualMemorySearchSetting, VirtualMemorySearchSetting> action)
    {
        var setting = action(new());

        ConcurrentBag<VirtualMemoryPointer<nint>> dict = new();

        Parallel.ForEach(process.Paging(setting).ToList(), address => process.SerachPage(address, setting, dict, (int value) => predicate(value)));

        return dict;
    }

    /// <summary>
    /// 搜索虚拟内存中满足指定谓词的数据
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer<nint>> SearchMemory(this Process process, Func<long, bool> predicate,
        Func<VirtualMemorySearchSetting, VirtualMemorySearchSetting> action)
    {
        var setting = action(new());

        ConcurrentBag<VirtualMemoryPointer<nint>> dict = new();

        Parallel.ForEach(process.Paging(setting).ToList(), address => process.SerachPage(address, setting, dict, (long value) => predicate(value)));

        return dict;
    }

    /// <summary>
    /// 搜索虚拟内存中满足指定谓词的数据(浮点数返回一个字典, 内存地址 + 初始的精确值)
    /// </summary>
    public static IEnumerable<KeyValuePair<VirtualMemoryPointer<nint>, float>> SearchMemory(this Process process, Func<float, bool> predicate,
        Func<VirtualMemorySearchSetting, VirtualMemorySearchSetting> action)
    {
        var setting = action(new());

        var dict = new ConcurrentDictionary<VirtualMemoryPointer<nint>, float>();

        Parallel.ForEach(process.Paging(setting).ToList(), address => process.SerachPage(address, setting, dict, (float value) => predicate(value)));

        return dict;
    }

    /// <summary>
    /// 搜索虚拟内存中满足指定谓词的数据(浮点数返回一个字典, 内存地址 + 初始的精确值)
    /// </summary>
    public static IEnumerable<KeyValuePair<VirtualMemoryPointer<nint>, double>> SearchMemory(this Process process, Func<double, bool> predicate,
        Func<VirtualMemorySearchSetting, VirtualMemorySearchSetting> action)
    {
        var setting = action(new());

        var dict = new ConcurrentDictionary<VirtualMemoryPointer<nint>, double>();

        Parallel.ForEach(process.Paging(setting).ToList(), address => process.SerachPage(address, setting, dict, (double value) => predicate(value)));

        return dict;
    }

    /// <summary>
    /// 搜索虚拟内存中满足指定谓词的数据
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer<nint>> SearchMemory(this Process process, Func<byte, bool> predicate)
        => SearchMemory(process, predicate, setting => setting);

    /// <summary>
    /// 搜索虚拟内存中满足指定谓词的数据
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer<nint>> SearchMemory(this Process process, Func<short, bool> predicate)
        => SearchMemory(process, predicate, setting => setting);

    /// <summary>
    /// 搜索虚拟内存中满足指定谓词的数据
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer<nint>> SearchMemory(this Process process, Func<int, bool> predicate)
        => SearchMemory(process, predicate, setting => setting);

    /// <summary>
    /// 搜索虚拟内存中满足指定谓词的数据
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer<nint>> SearchMemory(this Process process, Func<long, bool> predicate)
        => SearchMemory(process, predicate, setting => setting);

    /// <summary>
    /// 搜索虚拟内存中满足指定谓词的数据(浮点数返回一个字典, 内存地址 + 初始的精确值)
    /// </summary>
    public static IEnumerable<KeyValuePair<VirtualMemoryPointer<nint>, float>> SearchMemory(this Process process, Func<float, bool> predicate)
        => SearchMemory(process, predicate, setting => setting);

    /// <summary>
    /// 搜索虚拟内存中满足指定谓词的数据(浮点数返回一个字典, 内存地址 + 初始的精确值)
    /// </summary>
    public static IEnumerable<KeyValuePair<VirtualMemoryPointer<nint>, double>> SearchMemory(this Process process, Func<double, bool> predicate)
        => SearchMemory(process, predicate, setting => setting);

    #endregion
}


