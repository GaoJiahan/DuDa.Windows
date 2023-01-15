using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuDa.Windows.Extensions.VirtualMemory;
public static class VirtualMemorySearchExtension
{
    /// <summary>
    /// 搜索内存地址序列中满足目标字节数组的元素
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer> Search(this IEnumerable<VirtualMemoryPointer> result, byte[] target)
    {
        foreach (var item in result)
            if (item.Get<byte>(target.Length).SequenceEqual(target))
                yield return item;
    }


    /// <summary>
    /// 搜索内存地址序列中满足指定谓词的元素
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer> Search(this IEnumerable<VirtualMemoryPointer> result, Func<byte, bool> predicate)
    {
        foreach (var item in result)
            if (predicate(item.Get<byte>()))
                yield return item;
    }


    /// <summary>
    /// 搜索内存地址序列中满足指定谓词的元素
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer> Search(this IEnumerable<VirtualMemoryPointer> result, Func<short, bool> predicate)
    {
        foreach (var item in result)
            if (predicate(item.Get<short>()))
                yield return item;
    }


    /// <summary>
    /// 搜索内存地址序列中满足指定谓词的元素
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer> Search(this IEnumerable<VirtualMemoryPointer> result, Func<int, bool> predicate)
    {
        foreach (var item in result)
            if (predicate(item.Get<int>()))
                yield return item;
    }


    /// <summary>
    /// 搜索内存地址序列中满足指定谓词的元素
    /// </summary>
    public static IEnumerable<VirtualMemoryPointer> Search(this IEnumerable<VirtualMemoryPointer> result, Func<long, bool> predicate)
    {
        foreach (var item in result)
            if (predicate(item.Get<long>()))
                yield return item;
    }


    /// <summary>
    /// 搜索内存地址序列中满足指定谓词的元素
    /// </summary>
    public static IEnumerable<KeyValuePair<VirtualMemoryPointer, float>> Search(this IEnumerable<KeyValuePair<VirtualMemoryPointer, float>> result, Func<float, float, bool> predicate)
    {
        foreach (var item in result)
            if (predicate(item.Key.Get<float>(), item.Value))
                yield return item;
    }


    /// <summary>
    /// 搜索内存地址序列中满足指定谓词的元素
    /// </summary>
    public static IEnumerable<KeyValuePair<VirtualMemoryPointer, double>> Search(this IEnumerable<KeyValuePair<VirtualMemoryPointer, double>> result, Func<double, double, bool> predicate)
    {
        foreach (var item in result)
            if (predicate(item.Key.Get<double>(), item.Value))
                yield return item;
    }


    /// <summary>
    /// 搜索内存地址序列中满足数值的元素
    /// </summary>
    /// <remarks>
    /// 浮点数提供了一个精确值的默认搜索逻辑(模仿 CE 搜索), 如果不符合你的需求, 可以使用谓词的搜索方法<br/><br/>
    /// 如果没有小数, 范围在目标数加 1 到减 1 的范围内<br/><br/>
    /// 如果有小数, 则是小数点末位数 加 1 到减 1 的范围内
    /// </remarks>
    public static IEnumerable<KeyValuePair<VirtualMemoryPointer, float>> Search
        (this IEnumerable<KeyValuePair<VirtualMemoryPointer, float>> result, float target)
    {
        var (min, max) = target.Range();

        return result.Search((float value, float _) => value.Cmp(min, max));
    }


    /// <summary>
    /// 搜索内存地址序列中满足数值的元素
    /// </summary>
    /// <remarks>
    /// 浮点数提供了一个精确值的默认搜索逻辑(模仿 CE 搜索), 如果不符合你的需求, 可以使用谓词的搜索方法<br/><br/>
    /// 如果没有小数, 范围在目标数加 1 到减 1 的范围内<br/><br/>
    /// 如果有小数, 则是小数点末位数 加 1 到减 1 的范围内
    /// </remarks>
    public static IEnumerable<KeyValuePair<VirtualMemoryPointer, double>> Search
        (this IEnumerable<KeyValuePair<VirtualMemoryPointer, double>> result, double target)
    {
        var (min, max) = target.Range();

        return result.Search((double value, double _) => value.Cmp(min, max));
    }
}
