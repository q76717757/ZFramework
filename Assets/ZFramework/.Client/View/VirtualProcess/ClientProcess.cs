using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public class ClientProcess : VirtualProcess
    {
        public override void Start()
        {
            Log.Info(AssemblyLoader.CurrentBoot.GetDllName());

            Game.AddSingleComponent<ZEventTemp>();//临时给Zevent接一下update生命周期




            Log.Info("Client Process Start");
        }
    }

}
