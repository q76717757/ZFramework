using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public class SimpleProcess : Domain
    {
        protected override void OnStart()
        {
            Log.Info("SimpleProcess Start");

            Log.Info(NetHelper.GetIP());

            new Entity(this).AddComponent<TcpServerComponent>().StartServer();
        }
    }
}
