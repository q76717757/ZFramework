using System.Collections.Generic;
using ZFramework.EventType;

namespace ZFramework
{
    public class Launcher : EventCallback<EventType.LoadAssemblyFinish>
    {
        public override void Callback(LoadAssemblyFinish arg)
        {
            //var http = Game.Root.AddComponent<HttpServer>();

            //FOR -- VPS -> (VP.Awake)
            //Call Event VP Init Finish-->

            Log.Info("Game Start!");
        }
    }
}
