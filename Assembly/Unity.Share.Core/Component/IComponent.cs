using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    //将局部组件和全局组件的部分关键属性抽象出来一个接口,用于生命周期执行时方便进行统一处理
    public interface IComponent : IDispose
    {
        internal void AddEntityDependenceies(Entity entity);
        internal void RemoveEntityDependenceies();
    }
}
