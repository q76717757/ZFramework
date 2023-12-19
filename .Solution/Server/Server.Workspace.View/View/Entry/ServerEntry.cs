using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public class ServerEntry : Entry<ServerEntry>
    {
        public override void OnStart()
        {
            Log.Info("Server Start!");
        }
    }
}
