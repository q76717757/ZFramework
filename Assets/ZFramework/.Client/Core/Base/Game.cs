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
        internal AttributeMap AttributeMap = new AttributeMap();
        internal IdGenerater IdGenerater;
        internal TimeInfo TimeInfo;
        private Entity root;

        public static Entity Root
        {
            get 
            {
                return instance.root;
            }
        }

        private Game() { }//封闭构造 
        static IGameInstance CreateInstance()
        {
            instance = new Game();
            instance.TimeInfo = new TimeInfo();
            instance.IdGenerater = new IdGenerater(instance.TimeInfo);
            instance.root = Entity.CreateRoot();
            return instance;
        }

        void Load(Type[] allTypes)
        {
            AttributeMap.Load(allTypes);
            GameLoopSystem.Load(AttributeMap.GetTypesByAttribute(typeof(GameLoopAttribute)));
            EventSystem.Load(AttributeMap.GetTypesByAttribute(typeof(EventAttribute)));
        }

        void IGameInstance.Start(Type[] allTypes)
        {
            Load(allTypes);

            EventSystem.Call(new EventType.OnGameStart());//call -> Func/View
        }
        void IGameInstance.Reload(Type[] allTypes)//only server  
        {
            Load(allTypes);

            GameLoopSystem.Reload();//触发Reload生命周期
            EventSystem.Reload();//?  好像没用
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
            EventSystem.LateUpdate() ;
        }
        void IGameInstance.Close()
        {
            GameLoopSystem.Close();
            EventSystem.Close();
        }

        //公开方法
        public static Type[] GetTypesByAttribute(Type AttributeType)
        {
            return instance.AttributeMap.GetTypesByAttribute(AttributeType);
        }

    }
}