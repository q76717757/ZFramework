using System;

namespace ZFramework
{
    public sealed class Game : IGameInstance
    {
        private readonly static AttributeMap AttributeMap = new AttributeMap();
        internal readonly static GameLoopSystem GameLoopSystem = new GameLoopSystem();

        private readonly static TimeInfo TimeInfo = new TimeInfo();
        private static IdGenerater IdGenerater = new IdGenerater(TimeInfo);
        private static VirtualProcess[] virtualProcesses;

        private Game() { }//封闭构造

        //GameLoop
        void IGameInstance.Init(Type[] allTypes)
        {
            AttributeMap.Load(allTypes);
            GameLoopSystem.Load(GetTypesByAttribute<GameLoopAttribute>());
        }
        void IGameInstance.Start()
        {
            virtualProcesses = VirtualProcess.Load(GetTypesByAttribute<ProcessType>());
            foreach (var vp in virtualProcesses)
            {
                vp.Start();
            }
        }
        void IGameInstance.Reload(Type[] allTypes) 
        {
            (this as IGameInstance).Init(allTypes);
            GameLoopSystem.Reload();
        }
        void IGameInstance.Update()
        {
            TimeInfo.Update();
            GameLoopSystem.Update();
        }
        void IGameInstance.LateUpdate()
        {
            GameLoopSystem.LateUpdate();
        }
        void IGameInstance.Close()
        {
            foreach (var item in virtualProcesses)
            {
                item.Close();
            }
        }

        //public
        public static Type[] GetTypesByAttribute<T>() where T :BaseAttribute
        {
            return AttributeMap.GetTypesByAttribute<T>();
        }
        public static long GenerateInstanceId()
        {
            return IdGenerater.GenerateInstanceId();
        }


        internal static SingleComponent<T> AddSingleComponent<T>() where T : SingleComponent<T>
        {
            return virtualProcesses[0].Root.AddComponent<T>();
        }
        internal static void CallEnable(Component component)
        {
            GameLoopSystem.CallEnable(component);
        }
        internal static void CallDisable(Component component)
        {
            GameLoopSystem.CallDisable(component);
        }
    }
}