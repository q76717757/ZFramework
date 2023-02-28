using System;

namespace ZFramework
{
    interface ISingleComponent
    {
        void Set(object o);
        void UnSet();
    }

    /// <summary>
    /// 用Game.Add去添加全局单例组件 提供带参的Awake 就不自动实例了
    /// </summary>
    public abstract class SingleComponent<T> : Component , ISingleComponent where T:SingleComponent<T>
    {
        public static T Instance { get; private set; }

        void ISingleComponent.Set(object o)
        {
            if (Instance != null)
            {
                throw new Exception($"{typeof(T)}.Instance != null");
            }
            Instance = o as T;
        }
        void ISingleComponent.UnSet()
        {
            if (Instance != this)
            {
                throw new Exception($"{typeof(T)}.Instance != this");
            }
            Instance = null;
        }
    }
}
