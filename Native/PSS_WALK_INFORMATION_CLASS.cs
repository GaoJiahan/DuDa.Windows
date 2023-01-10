namespace DuDa.Windows.Native;

[Flags]
internal enum PSS_WALK_INFORMATION_CLASS : uint
{
    PSS_WALK_AUXILIARY_PAGES = 0,
    PSS_WALK_VA_SPACE = 1,
    PSS_WALK_HANDLES = 2,
    PSS_WALK_THREADS = 3
}
