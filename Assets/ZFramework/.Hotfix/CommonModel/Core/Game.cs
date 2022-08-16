using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class Game
    {
        public static GameLoop GameLoop { get;} = new GameLoop();

        private static VirtualProcess rootProcess;
        public static VirtualProcess RootProcess
        {
            get
            {
                if (rootProcess == null)
                {
                    rootProcess = new VirtualProcess();
                }
                return rootProcess;
            }
        }
    }

}
