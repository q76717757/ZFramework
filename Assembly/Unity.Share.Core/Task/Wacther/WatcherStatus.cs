using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    //观察者是一次性的  当他注视一个task之后  就不能再被其他task使用了  和task是强绑定关系
    public enum WatcherStatus
    {
        /// <summary>
        /// 未使用
        /// </summary>
        CanUse,
        /// <summary>
        /// 使用中
        /// </summary>
        Using,
        /// <summary>
        /// 过期
        /// </summary>
        Exp,
    }
}
