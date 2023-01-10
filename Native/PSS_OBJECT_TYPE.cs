﻿namespace DuDa.Windows.Native;
[Flags]
internal enum PSS_OBJECT_TYPE : uint
{
    PSS_OBJECT_TYPE_UNKNOWN = 0,
    PSS_OBJECT_TYPE_PROCESS = 1,
    PSS_OBJECT_TYPE_THREAD = 2,
    PSS_OBJECT_TYPE_MUTANT = 3,
    PSS_OBJECT_TYPE_EVENT = 4,
    PSS_OBJECT_TYPE_SECTION = 5,
    PSS_OBJECT_TYPE_SEMAPHORE = 6
}
