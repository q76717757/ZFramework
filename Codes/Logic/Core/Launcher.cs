using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public static class Launcher
    {
        public static async void Start()
        {
            Log.Info("Launcher Start!");

            Game.Root.AddComponent<HttpComponent>();
            Game.Root.AddComponent<BundleComponent>();

            var UI = Game.Root.AddComponent<UIComponent>();
            //UI.Show(UIType.View_Video);


            Log.Info("Launcher Start Finish!");
            //Game.Root.AddComponent<EventComponent>();
            //Game.Root.AddComponent<TimerComponent>();
            //Game.Root.AddComponent<AudioComponent>();
            {  //客户端热更新需要重启 不重启可能会引入bug?  关于对象残留
                //打开热更新UI //目标实现不重启更新
                //var configCOM = Game.Root.AddComponent<ConfigComponent>();
                //configCOM.LoadFromStreamingAssets();
                //var versionCOM = Game.Root.AddComponent<VersionComponent>();
                //var remoteVersion = await versionCOM.GetServerVersionAsync(configCOM.versionFileURL);
                //var localVersion = versionCOM.GetLocalVersion();
                //资源更新
                //重新反射程序集
            }
        }

    }
}
