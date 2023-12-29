using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    internal interface IGlobalComponent
    {
    }

    /// <summary>
    /// 全局组件不支持多层继承
    /// </summary>
    public abstract class GlobalComponent<T> : BasedComponent, IGlobalComponent where T : GlobalComponent<T>
    {
        private static T _instance;
        public static T Instance => _instance;
        protected GlobalComponent() { }

        protected internal sealed override void AddEntityDependenceies(Entity entity)
        {
            if (_instance != null)
            {
                throw new NotSupportedException("GlobalComponent只能添加一次");
            }
            _instance = this as T;
            base.AddEntityDependenceies(entity);
        }

        protected internal sealed override void RemoveEntityDependenceies()
        {
            base.RemoveEntityDependenceies();
            _instance = null;
        }
    }
}
