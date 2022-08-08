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

        private static Process root;
        public static Process Root
        {
            get
            {
                if (root == null)
                {
                    root = ProcessFactory.CreateProcess(ProcessType.Root);
                }
                return root;
            }
        }
    }

}
