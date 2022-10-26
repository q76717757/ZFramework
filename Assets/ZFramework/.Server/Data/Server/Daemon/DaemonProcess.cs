using System;

namespace ZFramework
{
    public class DaemonProcess : VirtualProcess
    {
        public override void Start()
        {

            Log.Info("Daemon Process Start");
        }
    }
}
