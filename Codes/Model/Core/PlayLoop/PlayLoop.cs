using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZFramework
{
    public sealed class PlayLoop : IEntry
    {
        public static PlayLoop Instance { get; private set; }

        //生命周期辅助对象映射表 //componentType - loopType - loopSystemObject
        private readonly Dictionary<Type, Dictionary<Type, List<IPlayLoopSystem>>> maps = new Dictionary<Type, Dictionary<Type, List<IPlayLoopSystem>>>();
        //特性-类型映射表
        private readonly static Dictionary<Type, List<Type>> attributeMap = new Dictionary<Type, List<Type>>();
        //所有组件集合
        private readonly Dictionary<long, Component> allComponts = new Dictionary<long, Component>();

        private Queue<long> updates = new Queue<long>();//用这个是为了不hold对象 防止删除失败跳出循环
        private Queue<long> updates2 = new Queue<long>();
        private Queue<long> lateUpdates = new Queue<long>();
        private Queue<long> lateUpdates2 = new Queue<long>();
        private Queue<long> destory = new Queue<long>();

        //private Queue<long> waitDeatory = new Queue<long>();
        //private Queue<long> waitDeatory2 = new Queue<long>();

        Assembly modelAssembly;
        Assembly codeAssembly;
        Type[] allTypes;

        void BuildAttributeMap(Type[] allTypes)
        {
            attributeMap.Clear();
            foreach (Type classType in allTypes)
            {
                if (classType.IsAbstract)
                {
                    continue;
                }
                object[] attributesObj = classType.GetCustomAttributes(typeof(BaseAttribute), true);
                if (attributesObj.Length == 0)
                {
                    continue;
                }

                foreach (BaseAttribute baseAttribute in attributesObj)
                {
                    Type AttributeType = baseAttribute.AttributeType;
                    if (!attributeMap.ContainsKey(AttributeType))
                    {
                        attributeMap.Add(AttributeType, new List<Type>());
                    }
                    attributeMap[AttributeType].Add(classType);
                }
            }
        }
        public static Type[] GetTypesByAttribute(Type AttributeType)
        {
            if (!attributeMap.ContainsKey(AttributeType))
            {
                attributeMap.Add(AttributeType, new List<Type>());
            }
            return attributeMap[AttributeType].ToArray();
        }
        void BuildPlayerLoopMaps()
        {
            maps.Clear();
            foreach (Type useLifeTypes in GetTypesByAttribute(typeof(LiveAttribute)))
            {
                object componentLiveSystemObj = Activator.CreateInstance(useLifeTypes);

                if (componentLiveSystemObj is IPlayLoopSystem iSystem)
                {
                    if (!maps.ContainsKey(iSystem.ComponentType))
                    {
                        maps.Add(iSystem.ComponentType, new Dictionary<Type, List<IPlayLoopSystem>>());
                    }
                    if (!maps[iSystem.ComponentType].ContainsKey(iSystem.PlayLoopType))
                    {
                        maps[iSystem.ComponentType][iSystem.PlayLoopType] = new List<IPlayLoopSystem>();
                    }
                    maps[iSystem.ComponentType][iSystem.PlayLoopType].Add(iSystem);
                }
            }
        }

        void IEntry.Start(Assembly model, Assembly logicAssembly)
        {
            Log.ILog = new UnityLogger();
            Instance = this;

            modelAssembly = model;
            var types = new List<Type>();
            types.AddRange(modelAssembly.GetTypes());
            types.AddRange(logicAssembly.GetTypes());
            allTypes = types.ToArray();

            BuildAttributeMap(allTypes);
            BuildPlayerLoopMaps();

            var met = logicAssembly.GetType("ZFramework.Launcher").GetMethod("Start");
            met.Invoke(null, new object[met.GetParameters().Length]);
        }
        void IEntry.Start(Assembly code)
        {
            Log.ILog = new UnityLogger();
            Instance = this;

            codeAssembly = code;
            allTypes = codeAssembly.GetTypes();

            BuildAttributeMap(allTypes);
            BuildPlayerLoopMaps();

            var met = codeAssembly.GetType("ZFramework.Launcher").GetMethod("Start");
            met.Invoke(null, new object[met.GetParameters().Length]);
        }
        void IEntry.Reload(Assembly logicAssembly)
        {
            var types = new List<Type>();
            types.AddRange(modelAssembly.GetTypes());
            types.AddRange(logicAssembly.GetTypes());
            allTypes = types.ToArray();

            BuildAttributeMap(allTypes);
            BuildPlayerLoopMaps();

            Reload();
        }

        void Reload()
        {
            //重建UpdateQueue 仅热重载需要 无中生有Update等延迟生命周期时 他们的id在Awake的时候没有入列 会导致Update遍历不到
            updates.Clear();
            updates2.Clear();
            lateUpdates.Clear();
            lateUpdates2.Clear();
            destory.Clear();

            foreach (var item in allComponts)
            {
                if (maps[item.Value.GetType()].ContainsKey(typeof(IUpdate)))
                {
                    updates.Enqueue(item.Key);
                }
                if (maps[item.Value.GetType()].ContainsKey(typeof(ILateUpdate)))
                {
                    lateUpdates.Enqueue(item.Key);
                }
                if (maps[item.Value.GetType()].ContainsKey(typeof(IDestory)))
                {
                    destory.Enqueue(item.Key);
                }
            }

            //reload生命周期回调
            foreach (var item in allComponts)
            {
                if (!allComponts.TryGetValue(item.Key, out Component component))
                {
                    continue;
                }
                if (component.IsDisposed)
                {
                    continue;
                }
                if (maps.TryGetValue(component.GetType(), out Dictionary<Type, List<IPlayLoopSystem>> life))
                {
                    if (life.TryGetValue(typeof(IReLoad), out List<IPlayLoopSystem> systemlist))
                    {
                        foreach (IReLoad system in systemlist)
                        {
                            try
                            {
                                system.Reload(component);
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
        public void Awake(Component component)
        {
            if (maps.TryGetValue(component.GetType(), out Dictionary<Type, List<IPlayLoopSystem>> iLifes))
            {
                if (iLifes.TryGetValue(typeof(IAwake), out List<IPlayLoopSystem> systems))
                {
                    foreach (IAwake system in systems)
                    {
                        if (system == null)
                        {
                            continue;
                        }
                        try
                        {
                            system.Awake(component);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }
        }
        public void Destroy(Component component)
        {
            if (maps.TryGetValue(component.GetType(), out Dictionary<Type, List<IPlayLoopSystem>> iLifes))
            {
                if (iLifes.TryGetValue(typeof(IDestory), out List<IPlayLoopSystem> systems))
                {
                    foreach (IDestory system in systems)
                    {
                        if (system == null)
                        {
                            continue;
                        }
                        try
                        {
                            system.Destory(component);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }
        }
        void IEntry.Update()
        {
            while (updates.Count > 0)
            {
                long instanceId = updates.Dequeue();
                if (!allComponts.TryGetValue(instanceId, out Component component))
                {
                    continue;
                }
                if (component.IsDisposed)
                {
                    continue;
                }

                if (maps.TryGetValue(component.GetType(), out Dictionary<Type, List<IPlayLoopSystem>> life))
                {
                    if (life.TryGetValue(typeof(IUpdate), out List<IPlayLoopSystem> systemlist))
                    {
                        updates2.Enqueue(instanceId);
                        foreach (IUpdate system in systemlist)
                        {
                            try
                            {
                                system.Update(component);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                            }
                        }
                    }
                }
            }
            (updates, updates2) = (updates2, updates);

            //延迟销毁, 帧末再进行销毁操作
            //while (waitDeatory.Count > 0)
            //{

            //}
            //(waitDeatory, waitDeatory2) = (waitDeatory2, waitDeatory);

        }
        void IEntry.LateUpdate()
        {
            while (lateUpdates.Count > 0)
            {
                long instanceId = lateUpdates.Dequeue();

                if (!allComponts.TryGetValue(instanceId, out Component component))
                {
                    continue;
                }
                if (component.IsDisposed)
                {
                    continue;
                }
                if (maps.TryGetValue(component.GetType(), out Dictionary<Type, List<IPlayLoopSystem>> life))
                {
                    if (life.TryGetValue(typeof(IUpdate), out List<IPlayLoopSystem> systemlist))
                    {
                        lateUpdates2.Enqueue(instanceId);

                        foreach (ILateUpdate system in systemlist)
                        {
                            try
                            {
                                system.LateUpdate(component);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                            }
                        }
                    }
                }
            }
            (lateUpdates, lateUpdates2) = (lateUpdates2, lateUpdates);
        }
        void IEntry.Close()
        {

        }



        //组件注册注销
        public bool AddComponentToPlayloop(Component component)
        {
            if (component == null || allComponts.ContainsKey(component.InstanceID))
            {
                return false;
            }
            allComponts.Add(component.InstanceID, component);


            if (maps[component.GetType()].ContainsKey(typeof(IUpdate)))
            {
                updates.Enqueue(component.InstanceID);//遍历全部component???? 还是判断一下不一定要全部入列
            }
            return true;
        }
        public bool RemoveComponentFromPlayloop(Component component)
        {
            if (component == null || !allComponts.ContainsKey(component.InstanceID))
            {
                return false;
            }
            allComponts.Remove(component.InstanceID);
            return true;
        }

    }

}
