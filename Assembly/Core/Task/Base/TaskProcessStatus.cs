using System;

namespace ZFramework
{
    /// <summary>
    /// 任务的过程状态
    /// </summary>
    public enum TaskProcessStatus : byte
    {
        /// <summary>
        /// 新建的实例,还未启动
        /// </summary>
        Created = 0,
        /// <summary>
        /// 执行中
        /// </summary>
        Running,
        /// <summary>
        /// 完结     (包含有4类完结情景  完成/取消/超时/异常)
        /// </summary>
        Completion,
    }
}
