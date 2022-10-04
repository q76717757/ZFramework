using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZFramework
{
    public sealed class Game : IGameInstance
    {
        internal static Game instance;
        internal GameLoopSystem GameLoopSystem = new GameLoopSystem();
        internal EventSystem EventSystem = new EventSystem();

        private readonly Dictionary<Type, List<Type>> attributeMap = new Dictionary<Type, List<Type>>();//特性-类型映射表

        private Game() { }//封闭构造
        static IGameInstance CreateInstance()
        {
            instance = new Game();
            return instance;
        }
        void Load(Type[] allTypes)
        {
            //特性扫描
            attributeMap.Clear();
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
                        attributeMap.Add(attribute.AttributeType, list);
                    }
                    list.Add(classType);
                }
            }
            GameLoopSystem.Load(GetTypesByAttribute(typeof(GameLoopAttribute)));
            EventSystem.Load(GetTypesByAttribute(typeof(EventAttribute)));
        }

        void IGameInstance.Start(Type[] allTypes)
        {
            Load(allTypes);
            EventSystem.Call(new EventType.OnGameStart());
        }
        void IGameInstance.Reload(Type[] allTypes)
        {
            Load(allTypes);
            GameLoopSystem.Reload();
            Log.Info("热重载");
        }
        void IGameInstance.Update()
        {
            GameLoopSystem.Update();
            EventSystem.Update();
        }
        void IGameInstance.LateUpdate()
        {
            GameLoopSystem.LateUpdate();
            EventSystem.LateUpdate();
        }
        void IGameInstance.Close()
        {
            GameLoopSystem.Close();
            EventSystem.Close();
        }

        //公开方法
        public static Type[] GetTypesByAttribute(Type AttributeType)
        {
            if (!instance.attributeMap.TryGetValue(AttributeType, out List<Type> list))
            {
                list = new List<Type>();
                instance.attributeMap.Add(AttributeType, list);
            }
            return list.ToArray();
        }

    }
}