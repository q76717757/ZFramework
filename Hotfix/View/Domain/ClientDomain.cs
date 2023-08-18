using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public class ClientDomain : Domain
    {
        protected override void OnStart()
        {
            Log.Info("ClientProcess Start");

            Application.targetFrameRate = 60;
            Screen.SetResolution(1920, 1080, true);

            new Entity(this).AddComponent<SceneComponent>();

            //new Entity(this).AddComponent<TcpClientComponent>().StartClient();

            //走热更新流程  loading场景 下载资源  提示重启

            //不需要更新

        }
    }
  
}
