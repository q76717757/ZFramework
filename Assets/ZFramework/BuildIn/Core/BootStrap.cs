using UnityEngine;
using System.Reflection;
using System;
using System.Collections;

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
    public enum CompileMode//编辑器下选择开发模式  使用热重载模式  发布后是无效枚举
    {
        Development,//开发模式
        Release,//发布模式
    }

    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public class BootStrap : MonoBehaviour
    {
        //public string GameName;//作为关键字   加载程序集   不同项目配置文件可以不同 由加载的程序集决定配置文件
                               //实现换内容的方式就是虚拟世界切换到空场景  (换空场景之前  需要确保bootstrap执行了close内容,需要确保手动清空所有的内容)
                               //空场景带有bootstrap引导   上面标记的gamename  从全局配置去下载dll并加载  下载的关键字就是这个key
                               //只有的逻辑全部由下载到的dll去决定

        public IEntry entry;
        public CompileMode mode;
        public BootFile boot;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            ET.ETTask.ExceptionHandler += (e) =>
            {
                Debug.Log(e.Message);
            };
        }

        void Start() => LoadGame();
        void Update() => entry.Update();
        void LateUpdate() => entry.LateUpdate();
        void OnApplicationQuit() => CloseGame();

        void LoadGame()
        {
            if (entry != null)
            {
                Debug.LogError("entry not null");
            }
            else
            {
                entry = AssemblyLoader.GetEntry("GameName", mode);
            }
        }
        void CloseGame()
        {
            if (entry != null)
            {
                entry.Close();
                entry = null;
            }
        }
    }

}
