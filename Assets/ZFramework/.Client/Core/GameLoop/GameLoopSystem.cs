using System;
using System.Collections.Generic;

namespace ZFramework
{
    internal sealed class GameLoopSystem
    {
        //生命周期映射表    ComponentType -- GameLoopType -- List<GameLoopImpl> 
        private readonly Dictionary<Type, Dictionary<Type, List<IGameLoop>>> gameloopMaps = new Dictionary<Type, Dictionary<Type, List<IGameLoop>>>();
        //   component.ID    component(instance)
        private readonly Dictionary<long, Component> allComponents = new Dictionary<long, Component>();

        private Queue<long> updates = new Queue<long>();
        private Queue<long> updates2 = new Queue<long>();
        private Queue<long> lateUpdates = new Queue<long>();
        private Queue<long> lateUpdates2 = new Queue<long>();
        private Queue<long> reload = new Queue<long>();
        private Queue<long> destory = new Queue<long>();

        internal void Load(Type[] allTypes)
        {
            gameloopMaps.Clear();
            foreach (Type type in allTypes)
            {
                if (Activator.CreateInstance(type) is IGameLoop gameloopObj)
                {
                    if (!gameloopMaps.TryGetValue(gameloopObj.ComponentType, out Dictionary<Type, List<IGameLoop>> systemMap))
                    {
                        systemMap = new Dictionary<Type, List<IGameLoop>>();
                        gameloopMaps.Add(gameloopObj.ComponentType, systemMap);
                    }
                   
                    if (!systemMap.TryGetValue(gameloopObj.GameLoopType,out List<IGameLoop> impls))
                    {
                        impls = new List<IGameLoop>();
                        systemMap.Add(gameloopObj.GameLoopType, impls);
                    }
                    impls.Add(gameloopObj);
                }
            }
        }
        internal void Update()
        {
            while (updates.Count > 0)
            {
                long id = updates.Dequeue();
                if (!allComponents.TryGetValue(id, out Component component) || component.IsDisposed)
                {
                    continue;
                }
                if (gameloopMaps.TryGetValue(component.GetType(), out Dictionary<Type, List<IGameLoop>> gameloopMap))
                {
                    if (gameloopMap.TryGetValue(typeof(IUpdate), out List<IGameLoop> gameloopImpls))
                    {
                        for (int i = 0; i < gameloopImpls.Count; i++)
                        {
                            try
                            {
                                (gameloopImpls[i] as IUpdate).OnUpdate(component);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                            }
                        }
                        updates2.Enqueue(id);
                    }
                }
            }
            (updates, updates2) = (updates2, updates);
        }
        internal void LateUpdate()
        {
            while (lateUpdates.Count > 0)
            {
                long id = lateUpdates.Dequeue();
                if (!allComponents.TryGetValue(id, out Component component) || component.IsDisposed)
                {
                    continue;
                }
                if (gameloopMaps.TryGetValue(component.GetType(), out Dictionary<Type, List<IGameLoop>> gameloopMap))
                {
                    if (gameloopMap.TryGetValue(typeof(ILateUpdate), out List<IGameLoop> gameloopImpls))
                    {
                        for (int i = 0; i < gameloopImpls.Count; i++)
                        {
                            try
                            {
                                (gameloopImpls[i] as ILateUpdate).OnLateUpdate(component);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                            }
                        }
                        lateUpdates2.Enqueue(id);
                    }
                }
            }
            (lateUpdates, lateUpdates2) = (lateUpdates2, lateUpdates);

            DelayDestroy();
        }
        void DelayDestroy()//延迟销毁 ??
        {
            
            while (destory.Count > 0)
            {
                long id = destory.Dequeue();
                if (!allComponents.TryGetValue(id, out Component component) || component.IsDisposed)
                {
                    continue;
                }
                if (gameloopMaps.TryGetValue(component.GetType(), out Dictionary<Type, List<IGameLoop>> gameloopMap))
                {
                    if (gameloopMap.TryGetValue(typeof(IDestory), out List<IGameLoop> gameloopImpls))
                    {
                        for (int i = 0; i < gameloopImpls.Count; i++)
                        {
                            try
                            {
                                (gameloopImpls[i] as IDestory).OnDestory(component);
                                allComponents.Remove(id);
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
        internal void Close()
        {
        }
        internal void Reload()
        {
            //重建UpdateQueue 仅热重载需要 无中生有Update等延迟生命周期时 他们的id在Awake的时候没有入列(不应该在awake入列?) 会导致Update遍历不到
            // 取消在awake上入列  改成全局扫描??  还是遍历所有entity 实时判断是否有update周期? 遍历所有效率低了点  重建列表好点
            updates.Clear();
            updates2.Clear();
            lateUpdates.Clear();
            lateUpdates2.Clear();
            reload.Clear();
            //destory.Clear();
            foreach (var item in allComponents)
            {
                var componentID = item.Key;
                var componentType = item.Value.GetType();

                if (gameloopMaps.TryGetValue(componentType,out Dictionary<Type,List<IGameLoop>> map))
                {
                    if (map.ContainsKey(typeof(IUpdate)))
                    {
                        updates.Enqueue(componentID);
                    }
                    if (map.ContainsKey(typeof(ILateUpdate)))
                    {
                        lateUpdates.Enqueue(componentID);
                    }
                    if (map.ContainsKey(typeof(IReLoad)))
                    {
                        reload.Enqueue(componentID);
                    }
                }
            }

            //reload生命周期回调
            while (reload.Count > 0)
            {
                long id = reload.Dequeue();
                if (!allComponents.TryGetValue(id, out Component component) || component.IsDisposed)
                {
                    continue;
                }
                if (gameloopMaps.TryGetValue(component.GetType(), out Dictionary<Type, List<IGameLoop>> gameloopMap))
                {
                    if (gameloopMap.TryGetValue(typeof(IReLoad), out List<IGameLoop> gameloopImpls))
                    {
                        for (int i = 0; i < gameloopImpls.Count; i++)
                        {
                            try
                            {
                                (gameloopImpls[i] as IReLoad).OnReload(component);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                            }
                        }
                    }
                }
            }
            Log.Info("<color=green>Hot Reload!</color>");
        }
        internal void CallAwake(Component component)
        {
            if (Register(component, out Dictionary<Type, List<IGameLoop>> gameLoopImpls))
            {
                if (gameLoopImpls.TryGetValue(typeof(IAwake), out List<IGameLoop> gameloopImpl))
                {
                    for (int i = 0; i < gameloopImpl.Count; i++)
                    {
                        try
                        {
                            (gameloopImpl[i] as IAwake).OnAwake(component);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }
            
        }
        internal void CallAwake<A>(Component component, A a)
        {
            if (Register(component, out Dictionary<Type, List<IGameLoop>> gameLoopImpls))
            {
                if (gameLoopImpls.TryGetValue(typeof(IAwake<A>), out List<IGameLoop> gameloopImpl))
                {
                    for (int i = 0; i < gameloopImpl.Count; i++)
                    {   
                        try
                        {
                            (gameloopImpl[i] as IAwake<A>).OnAwake(component, a);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }
        }
        internal void CallAwake<A, B>(Component component, A a, B b)
        {
            if (Register(component, out Dictionary<Type, List<IGameLoop>> gameLoopImpls))
            {
                if (gameLoopImpls.TryGetValue(typeof(IAwake<A, B>), out List<IGameLoop> gameloopImpl))
                {
                    for (int i = 0; i < gameloopImpl.Count; i++)
                    {
                        try
                        {
                            (gameloopImpl[i] as IAwake<A, B>).OnAwake(component, a, b);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }
            
        }
        internal void CallAwake<A, B, C>(Component component, A a, B b, C c)
        {
            if (Register(component, out Dictionary<Type, List<IGameLoop>> gameLoopImpls))
            {
                if (gameLoopImpls.TryGetValue(typeof(IAwake<A, B, C>), out List<IGameLoop> gameloopImpl))
                {
                    for (int i = 0; i < gameloopImpl.Count; i++)
                    {
                        try
                        {
                            (gameloopImpl[i] as IAwake<A, B, C>).OnAwake(component, a, b, c);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }
        }
        internal void CallEnable(Component component)
        {
            if (gameloopMaps.TryGetValue(component.GetType(), out Dictionary<Type, List<IGameLoop>> gameLoopImpls))
            {
                if (gameLoopImpls.TryGetValue(typeof(IEnable), out List<IGameLoop> gameloopImpl))
                {
                    for (int i = 0; i < gameloopImpl.Count; i++)
                    {
                        try
                        {
                            (gameloopImpl[i] as IEnable).OnEnable(component);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }
        }
        internal void CallDisable(Component component)
        {
            if (gameloopMaps.TryGetValue(component.GetType(),out Dictionary<Type,List<IGameLoop>> gameLoopImpls))
            {
                if (gameLoopImpls.TryGetValue(typeof(IDisable), out List<IGameLoop> gameloopImpl))
                {
                    for (int i = 0; i < gameloopImpl.Count; i++)
                    {
                        try
                        {
                            (gameloopImpl[i] as IDisable).OnDisable(component);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }
            
        }
        internal void CallDestory(Component component)
        {
            if (component == null)
            {
                return;
            }
            //递归把子物体和子物体上的component加进延迟销毁队列
            destory.Enqueue(component.InstanceID);
        }

        private bool Register(Component component, out Dictionary<Type, List<IGameLoop>> gameLoop)
        {
            if (component == null || allComponents.ContainsKey(component.InstanceID))
            {
                gameLoop = null;
                return false;
            }
            allComponents.Add(component.InstanceID, component);

            if (!gameloopMaps.TryGetValue(component.Type, out gameLoop))
            {
                return false;
            }

            if (gameLoop.ContainsKey(typeof(IUpdate)))
            {
                updates.Enqueue(component.InstanceID);
            }
            if (gameLoop.ContainsKey(typeof(ILateUpdate)))
            {
                lateUpdates.Enqueue(component.InstanceID);
            }
            if (gameLoop.ContainsKey(typeof(IReLoad)))
            {
                reload.Enqueue(component.InstanceID);
            }
            return true;
        }
    }
}
