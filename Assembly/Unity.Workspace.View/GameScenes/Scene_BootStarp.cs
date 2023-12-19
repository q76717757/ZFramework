using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZFramework
{
    public class Scene_BootStarp : GameScene
    {
        public override string SceneName => "BootStrap";

        public override void OnLoaded(LoadSceneMode mode)
        {
            Hotfix().Invoke();

            //var uiComponent =  DaschowEntry.Root.AddComponent<UIComponent>();
            //uiComponent.Init(DaschowEntry.RootCanvas);
            //uiComponent.ShowUI<Canvas_LoginIOS>();
        }

        public override void OnUnload()
        {
        }

        async ATask Hotfix()
        {
            //热更新分4种情景
            //1.只更新代码 不更新资源
            //2.只更新资源 不更新代码
            //3.代码和资源都更新了
            //4.代码和资源都不更新
            //如果涉及代码更新 需要重启  否则可以不重启,直接下载资源即可

            //跳转到热更新场景
            //然后再执行这些热更新的逻辑   一开始是加载1.0的代码和1.0的vfs,  切换到1.0的热更场景,  然后下载2.0的代码 2.0的资源  然后提示重启
            //重启后,直接读取的是2.0的代码和2.0的vfs配置

#if ENABLE_HYBRIDCLR && !UNITY_EDITOR
            bool enableHybridCLR = true;
#else
            bool enableHybridCLR = false;
#endif
            if (enableHybridCLR)
            {
                var reboot = await HotfixAssembly();
                await VirtualFileSystem.UpdateProfile();//更新资源清单

                if (reboot) 
                {
                    Log.Info("重启客户端");
                    return;
                }
            }

            Log.Info("正片开始-->");
            //进入整体
            UnityEntry.Root.GetComponent<SceneManagementComponent>().LoadScene("SceneBundle/guangzhoujiang", LoadSceneMode.Single);
        }

        async ATask<bool> HotfixAssembly()
        {
            IFileServer fileServer = new CosFileServer();
            //检查远端xml
            var xmlPath = $"{Defines.PROJECT_CODE}/{Defines.TargetRuntimePlatform}/Assembly/{HybridCLRUtility.HOTFIX_ASSEMBLY_XML}";
            if ((await fileServer.Exists(xmlPath)).Item1)//有远程程序集xml
            {
                //TODO  代码更新的过程 目前没有处理任何意外 如果更新过程中出现断网或者写入错误之类的  程序就会损坏
                //下载远程XML
                var xmlStr = await fileServer.DownloadFile(xmlPath);
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(Encoding.UTF8.GetString(xmlStr));

                //对比HASH
                var md5 = xmlDoc.SelectSingleNode("AssemblyInfo").Attributes["MD5"].Value;//远端hash

                if (HybridCLRUtility.AssemblyInfo.md5 != md5)//程序hash不一致 则存在更新 直接把最新的下下来
                {
                    //下载最新DLL
                    var dllPath = $"{Defines.PROJECT_CODE}/{Defines.TargetRuntimePlatform}/{HybridCLRUtility.DATA_FOLDER}/{HybridCLRUtility.HOTFIX_ASSEMBLY_BUNDLE}";
                    byte[] dll = await fileServer.DownloadFile(dllPath);

                    //保存到持久目录
                    Directory.CreateDirectory(Path.Combine(Defines.PersistenceDataAPath, HybridCLRUtility.DATA_FOLDER));
                    var dllSavePath = Path.Combine(Defines.PersistenceDataAPath, HybridCLRUtility.DATA_FOLDER, HybridCLRUtility.HOTFIX_ASSEMBLY_BUNDLE);
                    File.WriteAllBytes(dllSavePath, dll);

                    //保存最新xml
                    var xmlSavePath = Path.Combine(Defines.PersistenceDataAPath, HybridCLRUtility.DATA_FOLDER, HybridCLRUtility.HOTFIX_ASSEMBLY_XML);
                    File.WriteAllBytes(xmlSavePath, xmlStr);

                    Log.Info("代码更新完成");
                    return true;
                }
            }
            return false;
        }

    }
}
