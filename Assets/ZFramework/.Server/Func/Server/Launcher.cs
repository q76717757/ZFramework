using System.Collections.Generic;
using ZFramework.EventType;

namespace ZFramework
{
    public class Launcher : EventCallback<EventType.LoadAssemblyFinish>
    {
        public override void Callback(LoadAssemblyFinish arg)
        {
            //Load配置表
            //Add变体组件
            //根据配置表实例变体

            var vpm = Game.Root.AddComponent<VirtualProcessManager>();//singleComponent


            var http = Game.Root.AddComponent<HttpServer>();







            //FOR -- VPS -> (VP.Awake)
            //Call Event VP Init Finish-->





            Log.Info("Game Start!");
        }
    }
}
