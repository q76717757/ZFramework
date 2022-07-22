using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public static class Launcher
    {
        public static async void Start()
        {
            Log.ILog = new UnityLogger();
            Log.Info("<color=green>Launcher Start!</color>");

            //Game.Root.AddComponent<HttpComponent>();
            //Game.Root.AddComponent<BundleComponent>();

        }
    }
}
