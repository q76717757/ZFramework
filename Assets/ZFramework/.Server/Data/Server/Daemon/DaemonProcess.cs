using System;
using System.Text;

namespace ZFramework
{
    /// <summary> 守护进程 </summary>
    public class DaemonProcess : VirtualProcess
    {
        public override void Start()
        {
            Game.AddSingleComponent<TcpServerComponent>();
            Game.AddSingleComponent<TempDB>();//未接数据库  先用内存模拟
            Game.AddSingleComponent<ProtocolHandleComponent>();
            Game.AddSingleComponent<LoginServer>();
            Game.AddSingleComponent<MapServer>();
            //Game.AddSingleComponent<HttpTouch>();

            Log.Info("Daemon Process Start");

        }

        public override void Stop()
        {
        }
    }
}
