using DuDa.Windows.Extensions.VirtualMemory;
using DuDa.Windows.Native;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DuDa.Windows.Diagnostics;
internal class ThreadInfo : KernelObject
{
    public int Id { get; init; }

    public bool QueueAPC<T>(VirtualMemoryPointer<T> address, nint param) where T : unmanaged
    {
        using var handle = CopyHandle();

        if (!handle.IsInvalid) return false;

        if (!W32APC.QueueUserAPC(address, handle.DangerousGetHandle(), param))
        {
            Debug.WriteLine($"<{nameof(W32APC.QueueUserAPC)}> APC任务入队失败, 线程Id:{Id} - {Kernel32.GetLastError().GetLogMessage()}");

            return false;
        }

        return true;
    }

    public bool Suspend()
    {
        using var handle = CopyHandle();

        if (!handle.IsInvalid) return false;

        if (W32Thread.SuspendThread(handle.DangerousGetHandle()) is -1)
        {
            Debug.WriteLine($"<{nameof(W32Thread.SuspendThread)}> 增加线程挂起计数失败, 线程Id:{Id} - {Kernel32.GetLastError().GetLogMessage()}");

            return false;
        }

        return true;
    }

    public bool Resume()
    {
        using var handle = CopyHandle();

        if (!handle.IsInvalid) return false;

        if (W32Thread.ResumeThread(handle.DangerousGetHandle()) is -1)
        {
            Debug.WriteLine($"<{nameof(W32Thread.SuspendThread)}> 减少线程挂起计数失败, 线程Id:{Id} - {Kernel32.GetLastError().GetLogMessage()}");

            return false;
        }

        return true;
    }

    public bool Kill()
    {
        using var handle = CopyHandle();

        if (!handle.IsInvalid) return false;

        if (!W32Thread.TerminateThread(handle.DangerousGetHandle(), 0))
        {
            Debug.WriteLine($"<{nameof(W32Thread.TerminateThread)}> 杀死线程失败, 线程Id:{Id} - {Kernel32.GetLastError().GetLogMessage()}");

            return false;
        }

        return true;
    }


    //public bool HookEip()
    //{
    //    if (!Suspend()) return false;




    //    if (!Resume()) return false;

    //    return true;
    //}
}
