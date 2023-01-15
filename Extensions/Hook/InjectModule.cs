
using DuDa.Windows.Diagnostics;
using DuDa.Windows.Extensions.VirtualMemory;
using DuDa.Windows.Native;
using DuDa.Windows.Extensions.PE;
using PInvoke;
using System.Net;
using System.Text;
using static PInvoke.Kernel32;

namespace DuDa.Windows.Extensions.Hook;

public static class InjectModule
{
    public static void RemoteThreadInjectModule(this Process process, string path)
    {
        using var ptr = process.AllocMemory(256);

        ptr.Set(path, Encoding.ASCII);

        var LoadLibrary = process.Modules.Single(x => x.Name.ToLower() == "kernel32.dll").ExportFunctions.Single(func => func.Name == "LoadLibraryA").Address;

        using var hThread = process.CreateRemoteThread(LoadLibrary, ptr);

        WaitForSingleObject(hThread, -1);
    }

    internal static SafeObjectHandle CreateRemoteThread(this Process process, nint address, nint param, bool isSuspend = false)
    {
        var hThread = new SafeObjectHandle(W32Thread.CreateRemoteThread(process.Handle, 0, 0, address, param, isSuspend ? 4 : 0, out _));

        System.Diagnostics.Debug.WriteLineIf(hThread.IsInvalid, $"{nameof(CreateRemoteThread)} 创建 {process.Name} 进程远线程失败 - {GetLastError().GetLogMessage()}");

        return hThread;
    }

    public static void MemoryInjectModule(this Process process, string path)
    {
        var pe = PE.PE.Load(path);


    }


}
