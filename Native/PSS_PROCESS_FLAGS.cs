namespace DuDa.Windows.Native;

[Flags]
internal enum PSS_PROCESS_FLAGS :uint
{
    PSS_PROCESS_FLAGS_NONE = 0x00000000,
    PSS_PROCESS_FLAGS_PROTECTED = 0x00000001,
    PSS_PROCESS_FLAGS_WOW64 = 0x00000002,
    PSS_PROCESS_FLAGS_RESERVED_03 = 0x00000004,
    PSS_PROCESS_FLAGS_RESERVED_04 = 0x00000008,
    PSS_PROCESS_FLAGS_FROZEN = 0x00000010
}


