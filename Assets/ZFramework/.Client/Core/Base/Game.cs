using System;
using System.Collections.Generic;

namespace ZFramework
{
    public sealed class Game : IGameInstance
    {
        internal static Game instance;

        internal GameLoopSystem GameLoopSystem = new GameLoopSystem();
        internal EventSystem EventSystem = new EventSystem();
        internal AttributeMap AttributeMap = new AttributeMap();
        internal IdGenerater IdGenerater;//ID生成器
        internal TimeInfo TimeInfo = new TimeInfo();
        internal VirtualProcessManager vpm = new VirtualProcessManager();//虚拟进程

        private Game() { }//封闭构造 

        static IGameInstance CreateInstance()
        {
            instance = new Game();
            return instance;
        }
        void Load(Type[] allTypes)
        {
            AttributeMap.Load(allTypes);
            GameLoopSystem.Load(AttributeMap.GetTypesByAttribute(typeof(GameLoopAttribute)));
            EventSystem.Load(AttributeMap.GetTypesByAttribute(typeof(EventAttribute)));
        }

        //GameLoop
        void IGameInstance.Start(Type[] allTypes)
        {
            Load(allTypes);

            IdGenerater = new IdGenerater(TimeInfo);//依赖Time

            vpm.Start(AttributeMap.GetTypesByAttribute(typeof(ProcessType)));
        }
        void IGameInstance.Reload(Type[] allTypes)//only server  
        {
            Load(allTypes);

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
            //Destort root
            GameLoopSystem.Close();
            EventSystem.Close();
            instance = null;
        }

        //public
        public static Type[] GetTypesByAttribute(Type AttributeType)
        {
            return instance.AttributeMap.GetTypesByAttribute(AttributeType);
        }

        //Single
        public static T AddSingleComponent<T>() where T : SingleComponent<T>
        {
            var single = instance.vpm.singleVP.Root.AddComponent<T>();
            (single as ISingleComponent).Set(single);
            return single;
        }
        public static T AddSingleComponent<T, A>(A a) where T : SingleComponent<T>
        {
            var single = instance.vpm.singleVP.Root.AddComponent<T, A>(a);
            (single as ISingleComponent).Set(single);
            return single;
        }
        public static T AddSingleComponent<T, A, B>(A a, B b) where T : SingleComponent<T>
        {
            var single = instance.vpm.singleVP.Root.AddComponent<T, A, B>(a, b);
            (single as ISingleComponent).Set(single);
            return single;
        }
        public static T AddSingleComponent<T, A, B, C>(A a, B b, C c) where T : SingleComponent<T>
        {
            var single = instance.vpm.singleVP.Root.AddComponent<T, A, B, C>(a, b, c);
            (single as ISingleComponent).Set(single);
            return single;
        }
    }
}