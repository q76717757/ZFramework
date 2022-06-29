using UnityEngine;
using System;

namespace ZFramework
{
    //程序集定义
    //框架层   按依赖顺序
    //Boot  引导层  框架启动之前所需的内容 比如代码加载器 基础的AB包读取器 以及一些需要挂到unity预设上的脚本 如REF脚本,定位脚本,数据可视化/序列化脚本等 继承自MonoBehaviour的脚本
    //ThirdParty 第三方/工具层   一般放插件或者不依赖架构的独立模块

    //业务层  可以脱离unity引擎  可以给服务端复用
    //Model  业务数据层   只有数据定义 没有逻辑
    //Hotfix 业务逻辑层   只有逻辑  没有任何字段 一般是底层的算法或者工具类的内容

    //表现层 把这个层剥离可以给服务端复用 多端复用
    //ViewModel 表现数据层  和unity相关的定义 一般是UGUI组件对象
    //ViewHotfix 表现逻辑层  主要是UI动效和UI事件

    //Editor  编辑器拓展层  编辑器拓展脚本和插件的编辑器脚本都放在这个程序集内

    public enum RunMode
    {
        HotReload_EditorRuntime, //热重载模式
        Mono_RealMachine,//发布模式
    }

    [DisallowMultipleComponent]
    public class BootStrap : MonoBehaviour
    {
        public IEntry entry;
        public RunMode mode;

        private void Awake()
        {
            Define.RunMode = mode;
            DontDestroyOnLoad(gameObject);
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Debug.Log(e.ExceptionObject.ToString());
            };


            Debug.Log("Branch 2");
        }

        public void Start()
        {
            entry = AssemblyLoader.GetEntry(mode);
            if (entry != null)
            {
                entry.Start();
            }
            else
            {
                Debug.Log("Entry is Null");
                Destroy(this);
            }
        }
        private void Update() => entry.Update();
        private void LateUpdate() => entry.LateUpdate();
        private void OnApplicationQuit() => entry.Close();

    }

}