using System;
using System.Collections.Generic;

namespace ZFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class GameLoopAttribute : BaseAttribute
    {
    }

    /// <summary>
    /// 生命周期管理系统
    /// </summary>
    internal sealed class GameLoopSystem
    {
        //生命周期映射表    ComponentType -- GameLoopType -- List<GameLoopImpl> 
        private readonly Dictionary<Type, Dictionary<Type, List<IGameLoop>>> gameloopMaps = new Dictionary<Type, Dictionary<Type, List<IGameLoop>>>();
        //所有存活的组件    Component.ID    Component(instance)
        private readonly Dictionary<long, Component> aliveComponents = new Dictionary<long, Component>();
        //已经执行了awake 等待加入update/lateupdate队列
        private readonly List<Component> waitingAdd = new List<Component>();
        //被调了destory  等到帧末在执行destory生命周期
        private readonly Queue<ZObject> waitingRemove = new Queue<ZObject>();//包括entity和component

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
        internal void Update()
        {
            //Add
            if (waitingAdd.Count > 0)
            {
                for (int i = 0; i < waitingAdd.Count; i++)
                {
                    Component component = waitingAdd[i];
                    if (gameloopMaps.TryGetValue(component.GetType(), out Dictionary<Type, List<IGameLoop>> gameLoop))
                    {
                        if (gameLoop.ContainsKey(typeof(IUpdate)))//如果有这个列表,则至少有一种实现
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
                waitingAdd.Clear();
            }

            //Update
            if (updates.Count > 0)
            {
                while (updates.Count > 0)
                {
                    Component component = updates.Dequeue();
                    if (component != null)
                    {
                        CallUpdate(component);
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
                    Component component = lateupdate.Dequeue();
                    if (component != null)//已经销毁或者是本帧销毁
                    {
                        CallLateUpdate(component);
                        lateupdate2.Enqueue(component);
                    }
                }
                (lateupdate, lateupdate2) = (lateupdate2, lateupdate);
            }

            //Destory
            while (waitingRemove.Count > 0)
            {
                ZObject obj = waitingRemove.Dequeue();
                if (obj is Component component)
                {
                    CallDestory(component);
                }
                obj.DisposedFinish();
                aliveComponents.Remove(obj.InstanceID);
            }
        }

        internal void WaitingAdd(Component component)
        {
            waitingAdd.Add(component);
        }
        internal void WaitingDestory(Component component)
        {
            for (int i = 0; i < waitingAdd.Count; i++)
            {
                if (waitingAdd[i] == component)
                {
                    waitingAdd.RemoveAt(i);
                    break;
                }
            }
            waitingRemove.Enqueue(component);
        }
        internal void WaitingDestory(Entity entity)
        {
            waitingRemove.Enqueue(entity);
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
        private void CallUpdate(Component component)
        {
            if (GetGameLoops(component, typeof(IUpdate), out List<IGameLoop> gameloops))
            {
                if (component.Enable)
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
            }
        }
        private void CallLateUpdate(Component component)
        {
            if (GetGameLoops(component, typeof(ILateUpdate), out List<IGameLoop> gameloops))
            {
                if (component.Enable)
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
            }
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
