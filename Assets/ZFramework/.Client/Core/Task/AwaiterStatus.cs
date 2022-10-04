using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    internal enum AwaiterStatus : byte
    {
        /// <summary>未完成操作.</summary>
        Doing = 0,

        /// <summary>已完成操作.</summary>
        Finish = 1,

        /// <summary>因错误而终止操作.</summary>
        Fail = 2,

        /// <summary> 取消操作 </summary>
        Cancle = 3,
    }

    public static class ZTaskStatusExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsCompleted(this AwaiterStatus status)
        {
            return status != AwaiterStatus.Doing;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsCompletedSuccessfully(this AwaiterStatus status)
        {
            return status == AwaiterStatus.Finish;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsCanceled(this AwaiterStatus status)
        {
            return status == AwaiterStatus.Cancle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsFaulted(this AwaiterStatus status)
        {
            return status == AwaiterStatus.Fail;
        }
    }
}
