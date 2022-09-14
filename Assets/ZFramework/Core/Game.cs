using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class Game
    {
        public static GameLoopSystem GameLoop { get;} = new GameLoopSystem();
        public static EventSystem Event { get;} = new EventSystem();
    }

}
  