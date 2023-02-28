using System;

namespace ZFramework
{
    public sealed class Game : IGameInstance
    {
        //-----------临时写在这里  后面换个地方  //这些static到TimeInfo上
        public static DateTime ecoTime = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// epoch时间戳 13位 毫秒级
        /// </summary>
        public static long epochTick = ecoTime.Ticks / 10000;
#if true
        public const string InnerHost = "127.0.0.1";
        public const string Host = "127.0.0.1";
#else
        public const string InnerHost = "10.0.4.3";
        public const string Host = "81.69.248.107";
#endif
        public static long GetID() => instance.IdGenerater.GenerateInstanceId();
        //-------------------------------------------------------------

        internal static Game instance;

        internal AttributeMap AttributeMap = new AttributeMap();
        internal TimeInfo TimeInfo = new TimeInfo();
        internal IdGenerater IdGenerater;
        internal GameLoopSystem GameLoopSystem = new GameLoopSystem();
        internal EventSystem EventSystem = new EventSystem();
        private VirtualProcess[] virtualProcesses;

        private Game() { }//封闭构造

#if !SERVER
        [UnityEngine.Scripting.Preserve]
#endif
        static IGameInstance CreateInstance() => instance = new Game();

        void Load(Type[] allTypes)
        {
            AttributeMap.Load(allTypes);
            GameLoopSystem.Load(AttributeMap.GetTypesByAttribute<GameLoopAttribute>());
            EventSystem.Load(AttributeMap.GetTypesByAttribute<EventAttribute>());
        }

        //GameLoop
        void IGameInstance.Init(Type[] allTypes)
        {
            Load(allTypes);
            IdGenerater = new IdGenerater(TimeInfo);//依赖Time

            virtualProcesses = VirtualProcess.Load(AttributeMap.GetTypesByAttribute<ProcessType>());
            foreach (var vp in virtualProcesses)
            {
                vp.Start();
            }
        }
        void IGameInstance.Reload(Type[] allTypes)//only server  
        {
            Load(allTypes);
            //入口限定了调用时机 必然在帧间隔中
            GameLoopSystem.Reload();
        }
        void IGameInstance.Update()
        {
            TimeInfo.Update();
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
            foreach (var item in virtualProcesses)
            {
                item.Close();
            }
            instance = null;
        }

        //public
        public static Type[] GetTypesByAttribute<T>() where T :BaseAttribute
        {
            return instance.AttributeMap.GetTypesByAttribute<T>();
        }

        public static void AddSingleComponent<T>() where T : SingleComponent<T>
        {
            var single = instance.virtualProcesses[0].Root.AddComponent<T>();
            (single as ISingleComponent).Set(single);
        }
        public static void AddSingleComponent<T, A>(A a) where T : SingleComponent<T>
        {
            var single = instance.virtualProcesses[0].Root.AddComponent<T, A>(a);
            (single as ISingleComponent).Set(single);
        }
        public static void AddSingleComponent<T, A, B>(A a, B b) where T : SingleComponent<T>
        {
            var single = instance.virtualProcesses[0].Root.AddComponent<T, A, B>(a, b);
            (single as ISingleComponent).Set(single);
        }
        public static void AddSingleComponent<T, A, B, C>(A a, B b, C c) where T : SingleComponent<T>
        {
            var single = instance.virtualProcesses[0].Root.AddComponent<T, A, B, C>(a, b, c);
            (single as ISingleComponent).Set(single);
        }
    }
}