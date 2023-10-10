using System;
using System.Collections.Generic;

namespace ZFramework
{
    public sealed class Game : IGameInstance
    {
        private readonly static AttributeMapper AttributeMapper = new AttributeMapper();
        private readonly static TimeTickSystem TimeTickSystem = new TimeTickSystem();
        private readonly static IDGenerationSystem IDGenerationSystem = new IDGenerationSystem(TimeTickSystem);
        private readonly static GameLoopSystem GameLoopSystem = new GameLoopSystem();
        private readonly static DomainSystem DomainSystem = new DomainSystem();

        private IGameVisual gameVisual;//可视化组件

        private Game() { }//封闭构造

        //GameLoop
        void LoadAssembly(Type[] allTypes)
        {
            AttributeMapper.Load(allTypes);
            GameLoopSystem.Load(GetTypesByAttribute<GameLoopAttribute>());
        }
        void IGameInstance.Start(Type[] allTypes)
        {
            LoadAssembly(allTypes);
            DomainSystem.Start();
        }
        void IGameInstance.Reload(Type[] allTypes)
        {
            LoadAssembly(allTypes);
            GameLoopSystem.Reload();
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
            DomainSystem.Close();
        }


        internal static void WaitingAdd(Component component)
        {
            GameLoopSystem.WaitingAdd(component);
        }
        internal static void WaitingDestory(Component component)
        {
            GameLoopSystem.WaitingDestory(component);
        }
        internal static void WaitingDestory(Entity entity)
        { 
            GameLoopSystem.WaitingDestory(entity);
        }
        internal static void CallAwake(Component component)
        { 
            GameLoopSystem.CallAwake(component);
        }
        internal static void CallEnable(Component component)
        {
            GameLoopSystem.CallEnable(component);
        }
        internal static void CallDisable(Component component)
        {
            GameLoopSystem.CallDisable(component);
        }


        //Public static
        public static Type[] GetTypesByAttribute<T>() where T :BaseAttribute
        {
            return AttributeMapper.GetTypesByAttribute(typeof(T));
        }
        public static Domain GetDomain(int domainID)
        { 
            return DomainSystem.GetDomain(domainID);
        }
        public static void LoadDomain(Domain domain)
        {
            DomainSystem.LoadDomain(domain);
        }
        public static void UnloadDomain(int domainID)
        { 
            DomainSystem.UnloadDomain(domainID);
        }

    }
}