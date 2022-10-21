
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
}
