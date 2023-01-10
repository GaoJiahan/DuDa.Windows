namespace DuDa.Windows.Diagnostics;


public class ModuleInfo
{
    /// <summary>
    /// 模块句柄; 也是模块的加载地址
    /// </summary>
    required public long Handle { get; init; }

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

}

