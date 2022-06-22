using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class Game
    {
        private static Entity root;
        public static Entity Root {
            get {
                if (root == null)
                {
                    root = Entity.CreateEntity();
                }
                return root;
            }
        }

        public static UIComponent UI
        {
            get { 
                return root.GetComponent<UIComponent>();
            }
        }



    }
}
