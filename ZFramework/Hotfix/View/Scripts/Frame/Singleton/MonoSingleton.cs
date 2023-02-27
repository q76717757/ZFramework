using UnityEngine;

/// <summary>
/// Mono单例模板
/// </summary>
/// <typeparam name="T">需要单例化的组件类名</typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject Object = new GameObject(typeof(T).Name, new[] { typeof(T) });
                    DontDestroyOnLoad(Object);
                    _instance = Object.GetComponent<T>();
                    (_instance as IInitable)?.Init();
                }
                else
                {
                    //若实例已经存在,则进行提示
                    //Debug.LogWarning($"{_instance} already exists!");
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// 若继承组件单例的类需要实现Awake方法，需要在Awake方法最开始的地方调用一次base.Awake()，来给_instance赋值
    /// </summary>
    private void Awake()
    {
        _instance = this as T;
        DontDestroyOnLoad(this);
    }
}