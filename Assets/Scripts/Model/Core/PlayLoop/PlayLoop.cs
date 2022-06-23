using System;
using System.Collections.Generic;

namespace ZFramework
{
    public sealed class PlayLoop : IEntry
    {
        public static PlayLoop Instance { get; private set; }

        //所有组件集合
        private readonly Dictionary<long, Component> allComponts = new Dictionary<long, Component>();
        //生命周期辅助对象映射表 //componentType - loopType - loopSystemObject
        private readonly Dictionary<Type, Dictionary<Type, List<IPlayLoopSystem>>> maps = new Dictionary<Type, Dictionary<Type, List<IPlayLoopSystem>>>();

        private Queue<long> updates = new Queue<long>();
        private Queue<long> updates2 = new Queue<long>();
        private Queue<long> lateUpdates = new Queue<long>();
        private Queue<long> lateUpdates2 = new Queue<long>();
        private Queue<long> destory = new Queue<long>();

        void BuildPlayerLoopMaps()
        {
            maps.Clear();
            foreach (Type useLifeTypes in AssemblyLoader.GetTypesByAttribute(typeof(LiveAttribute)))
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

        void IEntry.Start()
        {
            Instance = this;
            BuildPlayerLoopMaps();
            AssemblyLoader.RunLauncher();
        }
        void IEntry.Reload()
        {
            BuildPlayerLoopMaps();
            AssemblyLoad();
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
                                throw;
                            }
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
                                throw;
                            }
                        }
                    }
                }
            }
            (lateUpdates, lateUpdates2) = (lateUpdates2, lateUpdates);

            ExecuteDestory();//延迟销毁
        }
        void IEntry.Focus(bool focus)
        {

        }
        bool IEntry.WantClose()
        {
            return true;
        }
        void IEntry.Close()
        {

        }

        public bool AddComponentToPlayloop(Component component)
        {
            if (component == null || allComponts.ContainsKey(component.InstanceID))
            {
                return false;
            }
            allComponts.Add(component.InstanceID, component);
            updates.Enqueue(component.InstanceID);
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

        void AssemblyLoad()
        {
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
                    if (life.TryGetValue(typeof(IAssemblyLoad), out List<IPlayLoopSystem> systemlist))
                    {
                        foreach (IAssemblyLoad system in systemlist)
                        {
                            try
                            {
                                system.AssemblyLoad(component);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                                throw;
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
        private void ExecuteDestory()
        {
            while (destory.Count > 0)
            {
                var id = destory.Dequeue();

            }
        }
    }

}
