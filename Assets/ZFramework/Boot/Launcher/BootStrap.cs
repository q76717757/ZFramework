using System;
using UnityEngine;

/*-*
 * 程序集的定义及设计原则
 * PC端可以有各种方法实现热更新 在此架构基础上完全可以热更一个文件更新器进去 然后用更新器对整个工程直接文件级替换更新 等于没有任何约束
 * 所以热更新设计主要针对手机平台
 * 
 * Boot       引导层       随工程发布一起发布  是启动整个架构的入口  **发布后不再修改 即不支持热更新
 * Core       核心层       整个架构最核心的模块封装在此  包括独立在MONO外的整个生命周期体系以及提供给各个模块相互通信的事件系统
 * Editor     编辑器拓展层 主要是一些编辑器工具 以及一些可视化面板  仅编辑器下有用
 * 
 * ---Data      公共数据层   实现双端代码共享的部分 与平台无关 不使用unityAPI的组件代码放在这里 服务端直接复用这部分代码
 * ---Func      公共逻辑层   实现双端代码共享的部分 与平台无关 不使用unityAPI的组件代码放在这里 服务端直接复用这部分代码
 * ---View      视图层   针对特定平台的客户端代码 使用了unityAPI的组件放在这里  通过修改视图层的代码可以实现逻辑共用  可通过重写view改成别的终端
 * 
 * Mono       挂载层       继承自MonoBehaviour的脚本放在这里  独立在一个程.序集内  使用wolong可以对这个进行热更  可以进行热更新 但不支持reload (需要重启)
 * 
 * Package    拓展包层     需要被脚本引用的插件放这里    这部分是外部插件不可控  不支持热更
 * Plugins    插件层       放一些平台特定库和动态库  这的部分加载由unity 处理的  不支持热更新
 */

namespace ZFramework
{ 
    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public sealed class BootStrap : MonoBehaviour
    {
        static BootStrap instance;
        [SerializeField]
        private BootFile boot;
        private DriveStrap drive;

        void Awake()
        {
            if (instance == null)
            { 
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }
        void Start() => StartGame(boot);
        void OnApplicationQuit() => CloseGame();

        public static void StartGame(BootFile boot)
        {
            if (instance == null)
            {
                var go = new GameObject("[BootStrap]");
                go.AddComponent<BootStrap>().boot = boot;
            }
            else
            {
                instance.drive = instance.gameObject.AddComponent<DriveStrap>();
                instance.drive.hideFlags = HideFlags.HideInInspector;
                instance.drive.StartGame(boot);
            }
        }
        public static void CloseGame()
        {
            if (instance != null)
            {
                instance.drive.CloseGame();
                Destroy(instance.gameObject);
                instance = null;
            }
        }
    }
}