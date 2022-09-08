using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZFramework
{
    public sealed class GameLoopSystem : IEntry
    {
        private readonly Dictionary<Type, Dictionary<Type, IGameLoop>> gameloopMaps = new Dictionary<Type, Dictionary<Type, IGameLoop>>();//生命周期辅助对象映射表 //componentType - loopType - loopSystemObject
        private readonly Dictionary<Type, List<Type>> attributeMap = new Dictionary<Type, List<Type>>();//特性-类型映射表
        private readonly Dictionary<long, ComponentData> allComponents = new Dictionary<long, ComponentData>();//所有组件(实体数据)集合

        private Queue<long> updates = new Queue<long>();
        private Queue<long> updates2 = new Queue<long>();
        private Queue<long> lateUpdates = new Queue<long>();
        private Queue<long> lateUpdates2 = new Queue<long>();
        private Queue<long> destory = new Queue<long>();

        Type[] componentTypeCache;

        void LoadAssembly(Type[] allTypes)
        {
            attributeMap.Clear();
            gameloopMaps.Clear();

            foreach (Type classType in allTypes)
            {
                if (classType.IsAbstract)
                {
                    continue;
                }
                foreach (BaseAttribute attribute in classType.GetCustomAttributes<BaseAttribute>(true))
                {
                    if (!attributeMap.TryGetValue(attribute.AttributeType,out List<Type> list))
                    {
                        list = new List<Type>();
                        attributeMap.Add(attribute.AttributeType, list);
                    }
                    list.Add(classType);
                }
            }

            foreach (Type useGameLoopType in GetTypesByAttribute(typeof(GameLoopAttribute)))
            {
                object gameloopSystemObj = Activator.CreateInstance(useGameLoopType);

                if (gameloopSystemObj is IGameLoop iSystem)
                {
                    if (!gameloopMaps.TryGetValue(iSystem.ComponentType,out Dictionary<Type, IGameLoop> systemMap))
                    {
                        systemMap = new Dictionary<Type, IGameLoop>();
                        gameloopMaps.Add(iSystem.ComponentType, systemMap);
                    }

                    if (!systemMap.ContainsKey(iSystem.GameLoopType))
                    {
                        systemMap.Add(iSystem.GameLoopType, iSystem);
                    }
                    else
                    {
                        //gameloop唯一?
                    }
                }
            }
        }
        public Type[] GetTypesByAttribute(Type AttributeType)
        {
            if (!attributeMap.TryGetValue(AttributeType, out List<Type> list))
            {
                list = new List<Type>();
                attributeMap.Add(AttributeType, list);
            }
            return list.ToArray();
        }

        //入口
        void IEntry.Load(Assembly dataAssembly, Assembly logicAssembly)
        {
            componentTypeCache = dataAssembly.GetTypes();
            var types = new List<Type>();
            types.AddRange(componentTypeCache);
            types.AddRange(logicAssembly.GetTypes());

            LoadAssembly(types.ToArray());

            var met = logicAssembly.GetType("ZFramework.Launcher").GetMethod("Start");
            met.Invoke(null, new object[met.GetParameters().Length]);
        }
        void IEntry.Load(Assembly code)
        {
            LoadAssembly(code.GetTypes());

            var met = code.GetType("ZFramework.Launcher").GetMethod("Start");
            met.Invoke(null, new object[met.GetParameters().Length]);
        }
        void IEntry.Reload(Assembly logicAssembly)
        {
            var types = new List<Type>();
            types.AddRange(componentTypeCache);
            types.AddRange(logicAssembly.GetTypes());

            LoadAssembly(types.ToArray());

            //重建UpdateQueue 仅热重载需要 无中生有Update等延迟生命周期时 他们的id在Awake的时候没有入列(不应该在awake入列?) 会导致Update遍历不到
            // 取消在awake上入列  改成全局扫描??  还是遍历所有entity 实时判断是否有update周期?

            updates.Clear();
            updates2.Clear();
            lateUpdates.Clear();
            lateUpdates2.Clear();
            destory.Clear();
            foreach (var item in allComponents)
            {
                var instanceId = item.Key;
                var entityType = item.Value.GetType();

                if (gameloopMaps.ContainsKey(entityType))
                {
                    if (gameloopMaps[entityType].ContainsKey(typeof(IUpdate)))
                    {
                        updates.Enqueue(instanceId);
                    }
                    if (gameloopMaps[entityType].ContainsKey(typeof(ILateUpdate)))
                    {
                        lateUpdates.Enqueue(instanceId);
                    }
                }
            }

            //reload生命周期回调
            foreach (var item in allComponents)
            {
                var instanceId = item.Key;
                var entity = item.Value;

                if (entity.IsDisposed)
                {
                    allComponents.Remove(instanceId);
                    continue;
                }
                if (gameloopMaps.TryGetValue(entity.GetType(), out Dictionary<Type, IGameLoop> life))
                {
                    if (life.TryGetValue(typeof(IReLoad), out IGameLoop systemlist) && systemlist is IReLoad system)
                    {
                        try
                        {
                            system.OnReload(entity);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }
            Log.Info("<color=green>Hot Reload!</color>");
        }

        //生命周期
        void IEntry.Update()
        {
            while (updates.Count > 0)
            {
                long instanceId = updates.Dequeue();
                if (!allComponents.TryGetValue(instanceId, out ComponentData component) || component.IsDisposed)
                {
                    continue;
                }
                if (gameloopMaps.TryGetValue(component.GetType(), out Dictionary<Type, IGameLoop> life))
                {
                    if (life.TryGetValue(typeof(IUpdate), out IGameLoop systemlist))
                    {
                        updates2.Enqueue(instanceId);
                        try
                        {
                            ((IUpdate)systemlist).OnUpdate(component);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }
            (updates, updates2) = (updates2, updates);
        }
        void IEntry.LateUpdate()
        {
            while (lateUpdates.Count > 0)
            {
                long instanceId = lateUpdates.Dequeue();
                if (!allComponents.TryGetValue(instanceId, out ComponentData component) || component.IsDisposed)
                {
                    continue;
                }
                if (gameloopMaps.TryGetValue(component.GetType(), out Dictionary<Type, IGameLoop> life))
                {
                    if (life.TryGetValue(typeof(ILateUpdate), out IGameLoop systemlist))
                    {
                        lateUpdates2.Enqueue(instanceId);
                        try
                        {
                            ((ILateUpdate)systemlist).OnLateUpdate(component);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e); 
                        }
                    }
                }
            }
            (lateUpdates, lateUpdates2) = (lateUpdates2, lateUpdates);

            //延迟销毁
            while (destory.Count > 0)
            {
                long instanceId = destory.Dequeue();
                if (!allComponents.TryGetValue(instanceId, out ComponentData component) || component.IsDisposed)
                {
                    continue;
                }
                if (gameloopMaps.TryGetValue(component.GetType(), out Dictionary<Type, IGameLoop> life))
                {
                    if (life.TryGetValue(typeof(IDestory), out IGameLoop systemlist))
                    {
                        if (systemlist is IDestory system)
                        {
                            try
                            {
                                system.OnDestory(component);
                                //entity.Dispose();
                                allComponents.Remove(instanceId);
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
        void IEntry.Close() 
        {

        }

        bool Register(ComponentData component, out Dictionary<Type, IGameLoop> gameLoop)
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
            return true;
        }
        public void CallAwake(ComponentData component)
        {
            if (!Register(component, out Dictionary<Type, IGameLoop> gameLoop))
            {
                return;
            }
            if (gameLoop.TryGetValue(typeof(IAwake), out IGameLoop systems))
            {
                try
                {
                    ((IAwake)systems).OnAwake(component);
                }
                catch (Exception e) { Log.Error(e); }
            }
        }
        public void CallAwake<A>(ComponentData component, A a)
        {
            if (!Register(component, out Dictionary<Type, IGameLoop> gameLoop))
            {
                return;
            }
            if (gameLoop.TryGetValue(typeof(IAwake<A>), out IGameLoop systems))
            {
                try
                {
                    ((IAwake<A>)systems).OnAwake(component, a);
                }
                catch (Exception e) { Log.Error(e); }
            }
        }
        public void CallAwake<A, B>(ComponentData component, A a, B b)
        {
            if (!Register(component, out Dictionary<Type, IGameLoop> gameLoop))
            {
                return;
            }
            if (gameLoop.TryGetValue(typeof(IAwake<A,B>), out IGameLoop systems))
            {
                try
                {
                    ((IAwake<A, B>)systems).OnAwake(component, a, b);
                }
                catch (Exception e) { Log.Error(e); }
            }
        }
        public void CallAwake<A, B, C>(ComponentData component, A a, B b, C c)
        { 

        }
        public void CallEnable(ComponentData component)
        {
            if (gameloopMaps.TryGetValue(component.Type, out Dictionary<Type, IGameLoop> iLifes))
            {
                if (iLifes.TryGetValue(typeof(IEnable), out IGameLoop systems))
                {
                    try
                    {
                        ((IEnable)systems).OnEnable(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
        public void CallDisable(ComponentData component)
        {
            if (gameloopMaps.TryGetValue(component.Type, out Dictionary<Type, IGameLoop> iLifes))
            {
                if (iLifes.TryGetValue(typeof(IDisable), out IGameLoop systems))
                {
                    try
                    {
                        ((IDisable)systems).OnDisable(component);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
        public void CallDestory(ComponentData component)
        {
            if (component == null)
            {
                return;
            }
            destory.Enqueue(component.InstanceID);
        }

    }

}
