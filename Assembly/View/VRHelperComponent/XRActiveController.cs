#if ENABLE_XR
using UnityEngine;
using UnityEngine.XR.Management;

namespace ZFramework
{
    /// <summary>
    /// 需要先初始化 再Start 头盔才可以显示画面
    /// 如果你勾选了 ProjectSettings- XR Plug-in Management 中的 Initialize XR On StartUp
    /// 项目运行时会自动初始化并调用XRManagerSettings中active的XRLoader.Start方法
    /// </summary>
    public class XRActiveController
    {
        private static XRManagerSettings ManagerSetting => XRGeneralSettings.Instance.Manager;
        public static bool InitializationComplete => ManagerSetting.isInitializationComplete;

        /// <summary>
        /// 如果是手动开关XR的,注意结束的时候要把XR关了
        /// </summary>
        public static void SetActive(bool state)
        {
            if (state)
            {
                Application.quitting += Application_quitting;
                if (!ManagerSetting.isInitializationComplete)
                {
                    ManagerSetting.InitializeLoaderSync();
                }
                if (!ManagerSetting.isInitializationComplete)
                {
                    Debug.LogError("初始化失败,需要先就绪XR环境");
                }
                else
                {
                    ManagerSetting.StartSubsystems();
                }
            }
            else
            {
                Application.quitting -= Application_quitting;
                if (ManagerSetting.isInitializationComplete)
                {
                    ManagerSetting.StopSubsystems();
                    ManagerSetting.DeinitializeLoader();
                }
            }
        }

        private static void Application_quitting()
        {
            SetActive(false);
        }
    }
}
#endif