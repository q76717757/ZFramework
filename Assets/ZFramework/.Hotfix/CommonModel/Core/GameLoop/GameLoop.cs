using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZFramework
{
    public sealed class GameLoop : IEntry
    {
        //生命周期辅助对象映射表 //componentType - loopType - loopSystemObject
        private readonly Dictionary<Type, Dictionary<Type, IGameLoopSystem>> gameloopSystemMaps = new Dictionary<Type, Dictionary<Type, IGameLoopSystem>>();
        //实体生命周期注册情况
        private readonly Dictionary<Type, EntityGameLoopUsing> gameloopUsing = new Dictionary<Type, EntityGameLoopUsing>();
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

        void LoadAssembly(Type[] allTypes)
        {
            attributeMap.Clear();
            gameloopSystemMaps.Clear();
            gameloopUsing.Clear();

            foreach (Type classType in allTypes)
            {
                if (classType.IsAbstract)
                {
                    continue;
                }
                foreach (BaseAttribute attribute in classType.GetCustomAttributes<BaseAttribute>(true))
                {
                    if (!attributeMap.TryGetValue(attribute.AttributeType, out List<Type> list))
                    {
                        list = new List<Type>();
                    }
                    list.Add(classType);
                }
            }

            foreach (Type useGameLoopType in GetTypesByAttribute(typeof(GameLoopAttribute)))
            {
                object gameloopSystemObj = Activator.CreateInstance(useGameLoopType);

                if (gameloopSystemObj is IGameLoopSystem iSystem)
                {
                    if (!gameloopSystemMaps.TryGetValue(iSystem.EntityType,out Dictionary<Type, IGameLoopSystem> systemMap))
                    {
                        systemMap = new Dictionary<Type, IGameLoopSystem>();
                    }

                    if (!systemMap.TryGetValue(iSystem.GameLoopType,out IGameLoopSystem system))
                    {
                        system = iSystem;
                    }
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

        //入口
        void IEntry.Load(Assembly model, Assembly logicAssembly)
        {
            modelTypeCache = model.GetTypes();
            var types = new List<Type>();
            types.AddRange(modelTypeCache);
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
            types.AddRange(modelTypeCache);
            types.AddRange(logicAssembly.GetTypes());

            LoadAssembly(types.ToArray());

            //重建UpdateQueue 仅热重载需要 无中生有Update等延迟生命周期时 他们的id在Awake的时候没有入列(不应该在awake入列?) 会导致Update遍历不到
            // 取消在awake上入列  改成全局扫描??  还是遍历所有entity 实时判断是否有update周期?

            updates.Clear();
            updates2.Clear();
            lateUpdates.Clear();
            lateUpdates2.Clear();
            destory.Clear();
            foreach (var item in allEntities)
            {
                var instanceId = item.Key;
                var entityType = item.Value.GetType();

                if (gameloopSystemMaps.ContainsKey(entityType))
                {
                    if (gameloopSystemMaps[entityType].ContainsKey(typeof(IUpdateSystem)))
                    {
                        updates.Enqueue(instanceId);
                    }
                    if (gameloopSystemMaps[entityType].ContainsKey(typeof(ILateUpdateSystem)))
                    {
                        lateUpdates.Enqueue(instanceId);
                    }
                }
            }

            //reload生命周期回调
            foreach (var item in allEntities)
            {
                var instanceId = item.Key;
                var entity = item.Value;

                if (entity.IsDisposed)
                {
                    allEntities.Remove(instanceId);
                    continue;
                }
                if (gameloopSystemMaps.TryGetValue(entity.GetType(), out Dictionary<Type, IGameLoopSystem> life))
                {
                    if (life.TryGetValue(typeof(IReLoadSystem), out IGameLoopSystem systemlist) && systemlist is IReLoadSystem system)
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
                if (!allEntities.TryGetValue(instanceId, out Entity entity) || entity.IsDisposed)
                {
                    continue;
                }
                if (gameloopSystemMaps.TryGetValue(entity.GetType(), out Dictionary<Type, IGameLoopSystem> life))
                {
                    if (life.TryGetValue(typeof(IUpdateSystem), out IGameLoopSystem systemlist))
                    {
                        updates2.Enqueue(instanceId);
                        try
                        {
                            ((IUpdateSystem)systemlist).OnUpdate(entity);
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
                if (!allEntities.TryGetValue(instanceId, out Entity entity) || entity.IsDisposed)
                {
                    continue;
                }
                if (gameloopSystemMaps.TryGetValue(entity.GetType(), out Dictionary<Type, IGameLoopSystem> life))
                {
                    if (life.TryGetValue(typeof(ILateUpdateSystem), out IGameLoopSystem systemlist))
                    {
                        lateUpdates2.Enqueue(instanceId);
                        try
                        {
                            ((ILateUpdateSystem)systemlist).OnLateUpdate(entity);
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
                if (!allEntities.TryGetValue(instanceId, out Entity entity) || entity.IsDisposed)
                {
                    continue;
                }
                if (gameloopSystemMaps.TryGetValue(entity.GetType(), out Dictionary<Type, IGameLoopSystem> life))
                {
                    if (life.TryGetValue(typeof(IDestorySystem), out IGameLoopSystem systemlist))
                    {
                        if (systemlist is IDestorySystem system)
                        {
                            try
                            {
                                system.OnDestory(entity);
                                //entity.Dispose();
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
        void IEntry.Close() 
        {

        }

        bool Register(Entity entity, out Dictionary<Type, IGameLoopSystem> gameLoop)
        {
            if (entity == null || allEntities.ContainsKey(entity.InstanceID))
            {
                gameLoop = null;
                return false;
            }
            allEntities.Add(entity.InstanceID, entity);

            if (!gameloopSystemMaps.TryGetValue(entity.GetType(), out gameLoop))
            {
                return false;
            }
            if (gameLoop.ContainsKey(typeof(IUpdateSystem)))
            {
                updates.Enqueue(entity.InstanceID);
            }
            if (gameLoop.ContainsKey(typeof(ILateUpdateSystem)))
            {
                lateUpdates.Enqueue(entity.InstanceID);
            }
            return true;
        }
        public void CallAwake(Entity entity)
        {
            if (!Register(entity, out Dictionary<Type, IGameLoopSystem> gameLoop))
            {
                return;
            }
            if (gameLoop == null)
            {
                return;
            }
            if (gameLoop.TryGetValue(typeof(IAwakeSystem), out IGameLoopSystem systems))
            {
                try
                {
                    ((IAwakeSystem)systems).OnAwake(entity);
                }
                catch (Exception e) { Log.Error(e); }
            }
        }
        public void CallAwake<A>(Entity entity, A a)
        {
            if (!gameloopSystemMaps.TryGetValue(entity.GetType(), out Dictionary<Type, IGameLoopSystem> gameLoop))
            {
                return;
            }
            if (gameLoop.TryGetValue(typeof(IAwakeSystem), out IGameLoopSystem systems))
            {
                try
                {
                    ((IAwakeSystem)systems).OnAwake(entity);
                }
                catch (Exception e) { Log.Error(e); }
            }
        }
        public void CallAwake<A, B>(Entity entity, A a, B b)
        {
            if (!gameloopSystemMaps.TryGetValue(entity.GetType(), out Dictionary<Type, IGameLoopSystem> gameLoop))
            {
                return;
            }
            if (gameLoop.TryGetValue(typeof(IAwakeSystem), out IGameLoopSystem systems))
            {
                try
                {
                    ((IAwakeSystem)systems).OnAwake(entity);
                }
                catch (Exception e) { Log.Error(e); }
            }
        }

        public void CallEnable(Entity entity)
        {
            if ((entity.GameLoopUsing & EntityGameLoopUsing.Enable) == EntityGameLoopUsing.Enable)
            {
                ((IEnableSystem)gameloopSystemMaps[entity.GetType()][typeof(IEnableSystem)]).OnEnable(entity);
            }

            if (gameloopSystemMaps.TryGetValue(entity.GetType(), out Dictionary<Type, IGameLoopSystem> iLifes))
            {
                if (iLifes.TryGetValue(typeof(IEnableSystem), out IGameLoopSystem systems))
                {
                    try
                    {
                        ((IEnableSystem)systems).OnEnable(entity);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
        public void CallDisable(Entity entity)
        {
            if (gameloopSystemMaps.TryGetValue(entity.GetType(), out Dictionary<Type, IGameLoopSystem> iLifes))
            {
                if (iLifes.TryGetValue(typeof(IDisableSystem), out IGameLoopSystem systems))
                {
                    try
                    {
                        ((IDisableSystem)systems as IDisableSystem).OnDisable(entity);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
        public void CallDestory(Entity entity)
        {
            if (entity == null)
            {
                return;
            }
            destory.Enqueue(entity.InstanceID);
        }

    }

}
