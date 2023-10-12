﻿using System;
using System.Collections.Generic;

namespace ZFramework
{
    public sealed class Game : IGameInstance
    {
        private readonly static AttributeMapper AttributeMapper = new AttributeMapper();
        private readonly static TimeTickSystem TimeTickSystem = new TimeTickSystem();
        private readonly static IDGenerationSystem IDGenerationSystem = new IDGenerationSystem(TimeTickSystem);
        private readonly static GameLoopSystem GameLoopSystem = new GameLoopSystem();
        internal readonly static Entity Root = new Entity(null);

        private IGameVisual gameVisual;//可视化组件

        private Game() { }//封闭构造

        //GameLoop
        void LoadAssembly(Type[] allTypes)
        {
            AttributeMapper.Load(allTypes);
            GameLoopSystem.Load();
        }
        void IGameInstance.Start(Type[] allTypes)
        {
            LoadAssembly(allTypes);
            Type entryType = GetTypesByAttribute<EntryAttribute>()[0];
            ((Entry)Activator.CreateInstance(entryType)).OnStart();
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
            Entity.DestroyImmediate(Root);
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
        internal static void DestoryObject(ZObject @object, bool isImmediate)
        {
            GameLoopSystem.DestoryObject(@object, isImmediate);
        }

        //Public static
        public static Type[] GetTypesByAttribute<T>() where T :BaseAttribute
        {
            return AttributeMapper.GetTypesByAttribute(typeof(T));
        }
    }
}