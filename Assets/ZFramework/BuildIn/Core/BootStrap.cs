using UnityEngine;
using System;
using System.Reflection;

namespace ZFramework
{
    public interface IEntry
    {
        void Start(Assembly code);
        void Start(Assembly model, Assembly logic);
        void Reload(Assembly logic);

        void Update();
        void LateUpdate();
        void Close();
    }

    public enum CompileMode
    {
        //可以热重载  本地有缓存目录
        Development,//开发模式(仅编辑器)
        //ab包构建工具可以选择debug还是release模式编译dll  编译完成自动移到asset目录下并且设置好ab包名?? 目前是在bootStrap上编译
        Release,//发布模式
    }

    [DisallowMultipleComponent]
    public class BootStrap : MonoBehaviour //连接unity的生命周期
    {
        public IEntry entry;
        public CompileMode mode;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            entry = AssemblyLoader.GetEntry(mode);
            if (entry == null)
            {
                Debug.LogError("Get Entry Fail");
                Destroy(this);
            }
        }

        void Update() => entry.Update();
        void LateUpdate() => entry.LateUpdate();
        void OnApplicationQuit() => entry.Close();

    }

}