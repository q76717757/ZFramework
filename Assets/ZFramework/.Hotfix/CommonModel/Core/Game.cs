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

        private static Process main;
        public static Process Main
        {
            get {
                if (main == null)
                {
                    main = ProcessFactory.CreateProcess();
                }
                return main;
            }
        }
    }

}
