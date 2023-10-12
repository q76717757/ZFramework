using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public class ServerEntry : Entry
    {
        public override void OnStart()
        {
            Log.ILog = new ConsoleLog();
            Log.Info("Server Start!");
        }

        public override void OnStop()
        {
            Log.Info("Server Stop!");
        }
    }
}
