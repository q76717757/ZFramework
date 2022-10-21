/** Header
 *  SingletonMono.cs
 *  泛型Mono单例  可重写Awake方法 => base.Awake(bool)  为true(默认)换场景不移除  为false换场景移除
 *  T不能嵌套泛型  因为嵌套泛型之后的类名和将没有对应的CS文件 无法挂载在gameobject上 会报错
 *  这个单例可以一开始就挂在场景的物体中 也可以通过代码直接调用 会自动生成 也可以重复挂 也可以多个场景都挂 但是只有第一个有效
 *  **/

using System;
using UnityEngine;

namespace ZFramework
{
    public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        private static T _instance;
        /// <summary> 实例 </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameObject($"[{typeof(T).Name}]").AddComponent<T>();
                return _instance;
            }
        }

        /// <summary> 等于Awake(true); </summary>
        public virtual void Awake()
        {
            Awake(true);
        }

#pragma warning disable UNT0006 // Incorrect message signature
        public void Awake(bool dontDestroy)
#pragma warning restore UNT0006 // Incorrect message signature
        {
            if (_instance == null)
            {
                if (dontDestroy)
                {
                    if (!transform.parent)
                    {
                        DontDestroyOnLoad(gameObject);
                    }
                    else
                    {
                        Log.Warning($"DontDestroyOnLoad Fail 不能作为子物体");
                    }
                }
                _instance = this as T;
            }
            else
            {
                if (_instance != this)
                {
                    Destroy(this);
                    throw new Exception($"已存在[{typeof(T).Name}]的实例");//抛异常是为了中断awake的执行
                }
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this) 
                _instance = null;
        }
    }

}
