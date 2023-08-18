using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace ZFramework
{
    /// <summary> 守护进程 </summary>
    public class DaemonProcess : Domain
    {
        protected override void OnStart()
        {
            //Game.AddSingleComponent<TcpServerComponent>();
            //Game.AddSingleComponent<HttpTouch>();

            Log.Info("Daemon Process Start");
        }
    }


}
