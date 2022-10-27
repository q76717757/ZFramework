using System;

namespace ZFramework
{
    /// <summary> 守护进程 </summary>
    public class DaemonProcess : VirtualProcess
    {
        public override void Start()
        {

            Log.Info("Daemon Process Start");
        }
    }
}
