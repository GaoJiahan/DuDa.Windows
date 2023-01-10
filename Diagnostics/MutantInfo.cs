namespace DuDa.Windows.Diagnostics;

public class MutantInfo : KernelObject
{
    /// <summary>
    /// 对象名称; 可能为 null
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// 当前计数
    /// </summary>
    public int CurrentCount { get; init; }

    /// <summary>
    /// 拥有线程退出而不释放互斥锁，则为 <see langword="true"/>; 否则为 <see langword="false"/>
    /// </summary>
    public bool Abandoned { get; init; }
}
