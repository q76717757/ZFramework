using System;

namespace ZFramework
{
    /// <summary>
    /// 四种任务完结的情景
    /// </summary>
    public enum TaskCompletionType : byte
    {
        /// <summary>
        /// 完成
        /// </summary>
        Success,
        /// <summary>
        /// 取消
        /// </summary>
        Cancel,
        /// <summary>
        /// 超时
        /// </summary>
        Timeout,
        /// <summary>
        /// 异常
        /// </summary>
        Exception,
    }


}
