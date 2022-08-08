using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class Game//他引用不到client层的东西  把game移到client层上???
    {
        public static GameLoop GameLoop { get;} = new GameLoop();

        private static World world;
        public static World World
        {
            get {
                if (world == null)
                { 
                    world = Entity.CreateEntity<World>();
                }
                return world;
            }
        }
    }

}
