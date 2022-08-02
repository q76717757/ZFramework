using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public class ResourcesAwake : AwakeSystem<ResourcesComponent>
    {
        public override void OnAwake(ResourcesComponent component)
        {
            Log.Info("ResourcesAwake");
        }
    }

    public static class ResourcesSystem
    {

        public static T Get<T>(this ResourcesComponent component, string path) where T:UnityEngine.Object
        {
            return Resources.Load<T>(path);
        }
    }
}
