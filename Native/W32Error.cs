using PInvoke;

namespace DuDa.Windows.Native;

internal static class W32LogExtension
{
    public static string GetLogMessage(this Win32ErrorCode err)
    {
        return $"错误信息:{err.GetMessage()}, 错误码:{err}({(uint)err})";
    }
}
