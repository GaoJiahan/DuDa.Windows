namespace DuDa.Windows.Native;
internal struct PSS_PERFORMANCE_COUNTERS
{
    public ulong TotalCycleCount;
    public ulong TotalWallClockPeriod;
    public ulong VaCloneCycleCount;
    public ulong VaCloneWallClockPeriod;
    public ulong VaSpaceCycleCount;
    public ulong VaSpaceWallClockPeriod;
    public ulong AuxPagesCycleCount;
    public ulong AuxPagesWallClockPeriod;
    public ulong HandlesCycleCount;
    public ulong HandlesWallClockPeriod;
    public ulong ThreadsCycleCount;
    public ulong ThreadsWallClockPeriod;
}