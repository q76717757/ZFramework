using System;
using System.Collections.Generic;

namespace ZFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class GameLoopAttribute : BaseAttribute
    {
    }

    internal sealed class GameLoopSystem
    {
        //生命周期映射表    ComponentType -- GameLoopType -- List<GameLoopImpl> 
        private readonly Dictionary<Type, Dictionary<Type, List<IGameLoop>>> gameloopMaps = new Dictionary<Type, Dictionary<Type, List<IGameLoop>>>();
        //所有存活的组件    Component.ID    Component(instance)
        private readonly Dictionary<long, Component> aliveComponents = new Dictionary<long, Component>();

        private List<Component> waitAddComponent = new List<Component>();//已经执行了awake 等待加入update队列
        private List<Component> waitRemoveComponent = new List<Component>();//被调了destory  等到帧末在执行生命周期

        private Queue<Component> updates = new Queue<Component>();
        private Queue<Component> updates2 = new Queue<Component>();
        private Queue<Component> lateupdate = new Queue<Component>();
        private Queue<Component> lateupdate2 = new Queue<Component>();

        internal void Load(Type[] allTypes)
        {
            gameloopMaps.Clear();
            foreach (Type type in allTypes)
            {
                if (Activator.CreateInstance(type) is IGameLoop gameloopObj)
                {
                    if (!gameloopMaps.TryGetValue(gameloopObj.ComponentType, out Dictionary<Type, List<IGameLoop>> componentMap))
                    {
                        componentMap = new Dictionary<Type, List<IGameLoop>>();
                        gameloopMaps.Add(gameloopObj.ComponentType, componentMap);
                    }
                   
                    if (!componentMap.TryGetValue(gameloopObj.GameLoopType,out List<IGameLoop> looptypeMap))
                    {
                        looptypeMap = new List<IGameLoop>();
                        componentMap.Add(gameloopObj.GameLoopType, looptypeMap);
                    }
                    looptypeMap.Add(gameloopObj);
                }
            }
        }
        internal void Reload()
        {
            updates.Clear();
            updates2.Clear();
            lateupdate.Clear();
            lateupdate2.Clear();
            List<Component> reloadComponents = new List<Component>();

            foreach (var item in aliveComponents)
            {
                //var componentID = item.Key;
                var component = item.Value;

                if (gameloopMaps.TryGetValue(component.GetType(), out Dictionary<Type, List<IGameLoop>> map))
                {
                    if (map.ContainsKey(typeof(IUpdate)))
                    {
                        updates.Enqueue(component);
                    }
                    if (map.ContainsKey(typeof(ILateUpdate)))
                    {
                        lateupdate.Enqueue(component);
                    }
                    if (map.ContainsKey(typeof(IReLoad)))
                    {
                        reloadComponents.Add(component);
                    }
                }
            }

            foreach (var item in reloadComponents)
            {
                CallReload(item);
            }
        }


        internal void AddComponent(Component component) => waitAddComponent.Add(component);
        internal void DestoryComponent(Component component) => waitRemoveComponent.Add(component);

        internal void Update()
        {
            //Add
            if (waitAddComponent.Count > 0)
            {
                //应该提前统一入队列?  不然在本帧添加的组件 本帧会直接执行update 原则上本帧ADD的组件 应该在下一帧才开始走update 本帧就同步走个Awake
                foreach (var component in waitAddComponent)
                {
                    if (gameloopMaps.TryGetValue(component.GetType(), out Dictionary<Type, List<IGameLoop>> gameLoop))
                    {
                        if (gameLoop.ContainsKey(typeof(IUpdate)))
                        {
                            updates.Enqueue(component);
                        }
                        if (gameLoop.ContainsKey(typeof(ILateUpdate)))
                        {
                            lateupdate.Enqueue(component);
                        }
                    }
                    aliveComponents.Add(component.InstanceID, component);
                }
                waitAddComponent.Clear();
            }

            //Update
            if (updates.Count > 0)
            {
                while (updates.Count > 0)
                {
                    var component = updates.Dequeue();
                    if (CallUpdate(component))
                    {
                        updates2.Enqueue(component);
                    }
                }
                (updates, updates2) = (updates2, updates);
            }
        }
        internal void LateUpdate()
        {
            //LateUpdate
            if (lateupdate.Count > 0)
            {
                while (lateupdate.Count > 0)
                {
                    var component = lateupdate.Dequeue();
                    if (CallLateUpdate(component))
                    {
                        lateupdate2.Enqueue(component);
                    }
                }
                (lateupdate, lateupdate2) = (lateupdate2, lateupdate);
            }

            //Destory
            if (waitRemoveComponent.Count > 0)
            {
                foreach (var item in waitRemoveComponent)
                {
                    CallDestory(item);
                    aliveComponents.Remove(item.InstanceID);
                    item.instanceID = 0;
                }
                waitRemoveComponent.Clear();
            }
        }


        bool GetGameLoops(Component component, Type gameloopType, out List<IGameLoop> gameloops)
        {
            if (gameloopMaps.TryGetValue(component.GetType(), out Dictionary<Type, List<IGameLoop>> gameloopImpls))
            {
                if (gameloopImpls.TryGetValue(gameloopType, out gameloops))
                {
                    return true;
                }
            }
            gameloops = null;
            return false;
        }
        internal void CallAwake(Component component)
        {
            if (GetGameLoops(component, typeof(IAwake), out List<IGameLoop> gameloops))
            {
                for (int i = 0; i < gameloops.Count; i++)
                {
                    try
                    {
                        (gameloops[i] as IAwake).OnAwake(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
        internal void CallEnable(Component component)
        {
            if (GetGameLoops(component, typeof(IEnable), out List<IGameLoop> gameloops))
            {
                for (int i = 0; i < gameloops.Count; i++)
                {
                    try
                    {
                        (gameloops[i] as IEnable).OnEnable(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
        internal void CallDisable(Component component)
        {
            if (GetGameLoops(component, typeof(IDisable), out List<IGameLoop> gameloops))
            {
                for (int i = 0; i < gameloops.Count; i++)
                {
                    try
                    {
                        (gameloops[i] as IDisable).OnDisable(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
        private void CallReload(Component component)
        {
            if (GetGameLoops(component, typeof(IReLoad), out List<IGameLoop> gameloops))
            {
                for (int i = 0; i < gameloops.Count; i++)
                {
                    try
                    {
                        (gameloops[i] as IReLoad).OnReload(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
        private bool CallUpdate(Component component)
        {
            if (GetGameLoops(component, typeof(IUpdate), out List<IGameLoop> gameloops))
            {
                if (component.Enable && !component.IsDisposed)
                {
                    for (int i = 0; i < gameloops.Count; i++)
                    {
                        try
                        {
                            (gameloops[i] as IUpdate).OnUpdate(component);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
                return true;
            }
            return false;
        }
        private bool CallLateUpdate(Component component)
        {
            if (GetGameLoops(component, typeof(ILateUpdate), out List<IGameLoop> gameloops))
            {
                if (component.Enable && !component.IsDisposed)
                {
                    for (int i = 0; i < gameloops.Count; i++)
                    {
                        try
                        {
                            (gameloops[i] as ILateUpdate).OnLateUpdate(component);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
                return true;
            }
            return false;
        }
        private void CallDestory(Component component)
        {
            if (GetGameLoops(component, typeof(IDestory), out List<IGameLoop> gameloops))
            {
                for (int i = 0; i < gameloops.Count; i++)
                {
                    try
                    {
                        (gameloops[i] as IDestory).OnDestory(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
    }
}
