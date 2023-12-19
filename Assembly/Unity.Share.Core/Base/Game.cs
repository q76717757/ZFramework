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
                    Log.Info($"{entryType.Name} Start");
                    IEntry entry = (IEntry)Activator.CreateInstance(entryType);
                    entry.OnStart(Root);
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

        internal static void CallAwake(IComponent component)
        {
            GameLoopSystem.CallAwake(component);
        }
        internal static void CallEnable(IComponent component)
        {
            GameLoopSystem.CallEnable(component);
        }
        internal static void CallDisable(IComponent component)
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
        public static T AddGlobalComponent<T>() where T : GlobalComponent<T>
        {
            return Root.AddComponentInner(typeof(T)) as T;
        }
    }
}