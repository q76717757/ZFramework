using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public sealed class UnityEntry : Entry
    {
        public override void OnStart()
        {
            Log.Info("Game Start!");

            Entity entity = new Entity();
            var child = entity.AddChild();
            var component = child.AddComponent<TestComponent>();
            //var component2 = component.GetComponent<SceneComponent>();
            //var components3 = component2.GetComponentInChildren<SceneComponent>();

            //entity.Parent = null;

            //Component.Destory(component);
            //Entity.Destory(entity);
        }
    }
}
