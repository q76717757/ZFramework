#region Header
/**
 *  SingletonCS.cs
 *  .Net对象泛型单例 实例时在返回前会调一次awake方法 按需可以重写之
 **/
#endregion

namespace ZFramework
{
    public abstract class SingletonCS<T> where T : SingletonCS<T>, new()
    {
        protected SingletonCS() {}
        private volatile static T _instance = null;
        private static readonly object _lock = new object();
        /// <summary> 实例 </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                            try
                            {
                                _instance.Awake();
                            }
                            catch (System.Exception e)
                            {
                                Log.Info(e.Message);
                            }
                        }
                    }
                }
                return _instance;
            }
        }
        /// <summary> 在实例对象后调用  </summary>
        protected virtual void Awake() { }

        //destory方法?
    }
}
