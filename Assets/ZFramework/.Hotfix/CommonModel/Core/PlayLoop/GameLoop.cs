using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZFramework
{
    public sealed class GameLoop : IEntry
    {
        //生命周期辅助对象映射表 //componentType - loopType - loopSystemObject
        private readonly Dictionary<Type, Dictionary<Type, List<IGameLoopSystem>>> maps = new Dictionary<Type, Dictionary<Type, List<IGameLoopSystem>>>();
        //特性-类型映射表
        private readonly Dictionary<Type, List<Type>> attributeMap = new Dictionary<Type, List<Type>>();
        //所有实体集合
        private readonly Dictionary<long, Entity> allEntities = new Dictionary<long, Entity>();

        private Queue<long> updates = new Queue<long>();
        private Queue<long> updates2 = new Queue<long>();
        private Queue<long> lateUpdates = new Queue<long>();
        private Queue<long> lateUpdates2 = new Queue<long>();
        private Queue<long> destory = new Queue<long>();

        Type[] modelTypeCache;

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
                    Type attributeType = baseAttribute.AttributeType;
                    if (!attributeMap.ContainsKey(attributeType))
                    {
                        attributeMap.Add(attributeType, new List<Type>());
                    }
                    attributeMap[attributeType].Add(classType);
                }
            }
        }
        public Type[] GetTypesByAttribute(Type AttributeType)
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
            foreach (Type useLifeTypes in GetTypesByAttribute(typeof(GameLoopAttribute)))
            {
                object componentLiveSystemObj = Activator.CreateInstance(useLifeTypes);

                if (componentLiveSystemObj is IGameLoopSystem iSystem)
                {
                    if (!maps.ContainsKey(iSystem.EntityType))
                    {
                        maps.Add(iSystem.EntityType, new Dictionary<Type, List<IGameLoopSystem>>());
                    }
                    if (!maps[iSystem.EntityType].ContainsKey(iSystem.PlayLoopType))
                    {
                        maps[iSystem.EntityType][iSystem.PlayLoopType] = new List<IGameLoopSystem>();
                    }
                    maps[iSystem.EntityType][iSystem.PlayLoopType].Add(iSystem);
                }
            }
        }

        //入口
        void IEntry.Start(Assembly model, Assembly logicAssembly)
        {
            modelTypeCache = model.GetTypes();
            var types = new List<Type>();
            types.AddRange(modelTypeCache);
            types.AddRange(logicAssembly.GetTypes());

            BuildAttributeMap(types.ToArray());
            BuildPlayerLoopMaps();

            var met = logicAssembly.GetType("ZFramework.Launcher").GetMethod("Start");
            met.Invoke(null, new object[met.GetParameters().Length]);
        }
        void IEntry.Start(Assembly code)
        {
            BuildAttributeMap(code.GetTypes());
            BuildPlayerLoopMaps();

            var met = code.GetType("ZFramework.Launcher").GetMethod("Start");
            met.Invoke(null, new object[met.GetParameters().Length]);
        }
        void IEntry.Reload(Assembly logicAssembly)
        {
            var types = new List<Type>();
            types.AddRange(modelTypeCache);
            types.AddRange(logicAssembly.GetTypes());

            BuildAttributeMap(types.ToArray());
            BuildPlayerLoopMaps();

            //重建UpdateQueue 仅热重载需要 无中生有Update等延迟生命周期时 他们的id在Awake的时候没有入列 会导致Update遍历不到
            updates.Clear();
            updates2.Clear();
            lateUpdates.Clear();
            lateUpdates2.Clear();
            destory.Clear();
            foreach (var item in allEntities)
            {
                var instanceId = item.Key;
                var entityType = item.Value.GetType();

                if (maps.ContainsKey(entityType))
                {
                    if (maps[entityType].ContainsKey(typeof(IUpdateSystem)))
                    {
                        updates.Enqueue(instanceId);
                    }
                    if (maps[entityType].ContainsKey(typeof(ILateUpdateSystem)))
                    {
                        lateUpdates.Enqueue(instanceId);
                    }
                }
            }

            //reload生命周期回调
            foreach (var item in allEntities)
            {
                var instanceId = item.Key;

                if (!allEntities.TryGetValue(instanceId, out Entity entity))
                {
                    continue;
                }
                if (entity.IsDisposed)
                {
                    allEntities.Remove(instanceId);
                    continue;
                }
                if (maps.TryGetValue(entity.GetType(), out Dictionary<Type, List<IGameLoopSystem>> life))
                {
                    if (life.TryGetValue(typeof(IReLoadSystem), out List<IGameLoopSystem> systemlist))
                    {
                        foreach (IReLoadSystem system in systemlist)
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
            }
            Log.Info("<color=green>Hot Reload!</color>");
        }

        //生命周期

        void IEntry.Update()
        {
            while (updates.Count > 0)
            {
                long instanceId = updates.Dequeue();
                if (!allEntities.TryGetValue(instanceId, out Entity entity))
                {
                    continue;
                }
                if (entity.IsDisposed)//改成==null?   好像没有必要
                {
                    continue;
                }
                if (maps.TryGetValue(entity.GetType(), out Dictionary<Type, List<IGameLoopSystem>> life))
                {
                    if (life.TryGetValue(typeof(IUpdateSystem), out List<IGameLoopSystem> systemlist))
                    {
                        updates2.Enqueue(instanceId);
                        foreach (IUpdateSystem system in systemlist)
                        {
                            try
                            {
                                system.OnUpdate(entity);
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
        }
        void IEntry.LateUpdate()
        {
            while (lateUpdates.Count > 0)
            {
                long instanceId = lateUpdates.Dequeue();
                if (!allEntities.TryGetValue(instanceId, out Entity entity))
                {
                    continue;
                }
                if (entity.IsDisposed)
                {
                    continue;
                }
                if (maps.TryGetValue(entity.GetType(), out Dictionary<Type, List<IGameLoopSystem>> life))
                {
                    if (life.TryGetValue(typeof(ILateUpdateSystem), out List<IGameLoopSystem> systemlist))
                    {
                        lateUpdates2.Enqueue(instanceId);
                        foreach (ILateUpdateSystem system in systemlist)
                        {
                            try
                            {
                                system.OnLateUpdate(entity);
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

            Destory();//延迟销毁
        }
        void Destory()
        {
            while (destory.Count > 0)
            {
                long instanceId = destory.Dequeue();
                if (!allEntities.TryGetValue(instanceId, out Entity entity))
                {
                    continue;
                }
                if (entity.IsDisposed)
                {
                    continue;
                }
                if (maps.TryGetValue(entity.GetType(), out Dictionary<Type, List<IGameLoopSystem>> life))
                {
                    if (life.TryGetValue(typeof(IDestorySystem), out List<IGameLoopSystem> systemlist))
                    {
                        foreach (IDestorySystem system in systemlist)//这个生命周期一般就只有一个 考虑把列表去掉?
                        {
                            try
                            {
                                system.OnDestory(entity);
                                entity.InstanceID = 0;
                                allEntities.Remove(instanceId);
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
        void IEntry.Close() { }


        void CallAwake(Entity entity)
        {
            if (maps.TryGetValue(entity.GetType(), out Dictionary<Type, List<IGameLoopSystem>> iLifes))
            {
                if (iLifes.TryGetValue(typeof(IAwakeSystem), out List<IGameLoopSystem> systems))
                {
                    foreach (IAwakeSystem system in systems)
                    {
                        try
                        {
                            system.OnAwake(entity);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }
        }

        public void RegisterEntityToPlayloop(Entity entity)
        {
            if (entity == null || allEntities.ContainsKey(entity.InstanceID))
            {
                return;
            }
            allEntities.Add(entity.InstanceID, entity);

            if (!maps.ContainsKey(entity.GetType()))
            {
                return;
            }
            if (maps[entity.GetType()].ContainsKey(typeof(IUpdateSystem)))
            {
                updates.Enqueue(entity.InstanceID);
            }
            if (maps[entity.GetType()].ContainsKey(typeof(ILateUpdateSystem)))
            {
                lateUpdates.Enqueue(entity.InstanceID);
            }
            CallAwake(entity);
        }//临时的UI有需要把生命周期注册曝露出来
        internal void RemoveEntityFromPlayloop(Entity entity)
        {
            if (entity == null)
            {
                return;
            }
            destory.Enqueue(entity.InstanceID);
        }
    }

}
