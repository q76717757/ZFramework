using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZFramework
{
    public sealed class Game : IGameInstance
    {
        internal static Game instance;
        internal GameLoopSystem GameLoopSystem;
        internal EventSystem EventSystem;
        private Dictionary<Type, List<Type>> attributeMap = new Dictionary<Type, List<Type>>();//特性-类型映射表

        private Game() { }//封闭构造
        static IGameInstance CreateInstance()
        {
            instance = new Game();
            return instance;
        }

        void LoadAttribute(Type[] allTypes)
        {
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
        }
        void Start()
        {
            //载入生命周期系统
            GameLoopSystem = new GameLoopSystem();
            GameLoopSystem.Load(GetTypesByAttribute(typeof(GameLoopAttribute)));

            //载入事件系统
            EventSystem = new EventSystem();
            EventSystem.Load(GetTypesByAttribute(typeof(EventAttribute)));

            //装载完成  派事件出去  View层自己去订阅 自己处理
            EventSystem.Call(new EventType.OnGameStart());
        }

        void IGameInstance.Load(Assembly code)
        {
            LoadAttribute(code.GetTypes());
            Start();
        }
        void IGameInstance.Load(Assembly data, Assembly func)
        {
            List<Type> types = new List<Type>();
            types.AddRange(data.GetTypes());
            types.AddRange(func.GetTypes());
            LoadAttribute(types.ToArray());
            Start();
        }
        void IGameInstance.Reload(Assembly func)
        {
            //GameLoop.Reload(func);
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
            GameLoopSystem = null;
            EventSystem = null;
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