using System;

namespace ZFramework
{
    enum ATaskStatus : byte
    {
        //任务的过程状态
        /// <summary>
        /// 创建
        /// </summary>
        Created = 0,
        /// <summary>
        /// 执行
        /// </summary>
        Running,

        //四种任务终结的情景
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
        /// 错误
        /// </summary>
        Faulted,
    }

    static class ATaskStatusUtility
    {
        //这个方法的返回直接决定到异步任务在遇到Await关键字时是等待还是继续执行
        internal static bool IsCompeleted(this ATaskStatus state)
        {
            switch (state)
            {
                case ATaskStatus.Created:
                case ATaskStatus.Running:
                    return false;
                case ATaskStatus.Success:
                case ATaskStatus.Cancel:
                case ATaskStatus.Timeout:
                case ATaskStatus.Faulted:
                    return true;
                default:throw new NotImplementedException($"ATaskStatus.IsCompeleted有未实现的状态:{state}");
            }
        }
    }
}
