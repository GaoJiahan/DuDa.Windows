namespace DuDa.Windows.Diagnostics;

public class EventInfo : KernelObject
{
    /// <summary>
    /// 对象名称; 可能为 null
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// 如果事件是手动重置的，则为 <see langword="true"/>，如果不是，则为 <see langword="false"/>
    /// </summary>
    public bool ManualReset { get; init; }


    /// <summary>
    /// 如果事件当前状态为触发, 则为 <see langword="true"/>，如果不是，则为 <see langword="false"/>
    /// </summary>
    public bool Signaled { get; init; }
}
