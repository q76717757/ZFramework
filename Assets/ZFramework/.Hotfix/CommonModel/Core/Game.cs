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

        private static Process rootProcess;
        public static Process RootProcess
        {
            get
            {
                if (rootProcess == null)
                {
                    rootProcess = new Process();
                }
                return rootProcess;
            }
        }
    }

}
