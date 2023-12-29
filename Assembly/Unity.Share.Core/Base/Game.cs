using System;
using System.Collections.Generic;

namespace ZFramework
{
    public sealed class Game : IGameInstance
    {
        private readonly static AttributeMapper AttributeMapper = new AttributeMapper();
        private readonly static TimeTickSystem TimeTickSystem = new TimeTickSystem();
        private readonly static GameLoopSystem GameLoopSystem = new GameLoopSystem();
        private readonly static Entity Root = new Entity(null);//全局根节点

        private IGameVisual gameVisual;//可视化组件

        private Game() { }//封闭构造

        //GameLoop
        void IGameInstance.Start(IList<Type> allTypes)
        {
            AttributeMapper.Load(allTypes);

            foreach (Type entryType in GetTypesByAttribute<EntryAttribute>())
            {
                try
                {
                    ((IEntry)Activator.CreateInstance(entryType)).OnStart(Root);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        void IGameInstance.Update()
        {
            TimeTickSystem.Update();
            GameLoopSystem.Update();
        }
        void IGameInstance.LateUpdate()
        {
            GameLoopSystem.LateUpdate();
        }
        void IGameInstance.Close()
        {
            ZObject.DestroyImmediate(Root);
        }

        internal static void CallAwake(BasedComponent component)
        {
            GameLoopSystem.CallAwake(component);
        }
        internal static void CallAwake<A>(BasedComponent component, A a)
        {
            GameLoopSystem.CallAwake(component, a);
        }
        internal static void CallAwake<A, B>(BasedComponent component, A a, B b)
        {
            GameLoopSystem.CallAwake(component, a, b);
        }
        internal static void CallAwake<A, B, C>(BasedComponent component, A a, B b, C c)
        {
            GameLoopSystem.CallAwake(component, a, b, c);
        }
        internal static void CallEnable(BasedComponent component)
        {
            GameLoopSystem.CallEnable(component);
        }
        internal static void CallDisable(BasedComponent component)
        {
            GameLoopSystem.CallDisable(component);
        }
        internal static void DestoryHandler(IDispose despose, bool isImmediate)
        {
            GameLoopSystem.DestoryHandler(despose, isImmediate);
        }


        //Public static
        public static Type[] GetTypesByAttribute<T>() where T :BaseAttribute
        {
            return AttributeMapper.GetTypesByAttribute(typeof(T));
        }

        private static bool TryGetGlobalComponent<T>(out T component) where T : GlobalComponent<T>
        {
            T instance = GlobalComponent<T>.Instance;
            component = instance;
            return component != null;
        }
        public static T AddGlobalComponent<T>() where T : GlobalComponent<T>
        {
            if (TryGetGlobalComponent(out T component))
            {
                return component;
            }
            else
            {
                return Root.AddChild(typeof(T).Name).AddComponentInner(typeof(T), true) as T;
            }
        }
        public static T AddGlobalComponent<T, A>(A a) where T : GlobalComponent<T>
        {
            if (TryGetGlobalComponent(out T component))
            {
                return component;
            }
            else
            {
                return Root.AddChild(typeof(T).Name).AddComponentInner(typeof(T), true, a) as T;
            }
        }
        public static T AddGlobalComponent<T, A, B>(A a, B b) where T : GlobalComponent<T>
        {
            if (TryGetGlobalComponent(out T component))
            {
                return component;
            }
            else
            {
                return Root.AddChild(typeof(T).Name).AddComponentInner(typeof(T), true, a, b) as T;
            }
        }
        public static T AddGlobalComponent<T, A, B, C>(A a, B b, C c) where T : GlobalComponent<T>
        {
            if (TryGetGlobalComponent(out T component))
            {
                return component;
            }
            else
            {
                return Root.AddChild(typeof(T).Name).AddComponentInner(typeof(T), true, a, b, c) as T;
            }
        }

    }
}