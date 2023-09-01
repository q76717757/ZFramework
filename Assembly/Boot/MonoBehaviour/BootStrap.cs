using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public sealed class BootStrap : MonoBehaviour
    {
        static IGameInstance game;

        void Start()
        {
            ZLogVisualization.Visua = true;
            if (game != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            game = StartGame();
        }

        IGameInstance StartGame()
        {
            //根据引导配置预加载程序集
            if (BootProfile.GetInstance().IsEnableHotfixCore)
            {
                HybridCLRUtility.LoadAssembly();
            }
            //遍历域中所有程序集,获取框架管理的程序集
            List<Type> allTypes = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (Defines.AssemblyNames.Contains(assembly.GetName().Name))
                {
                    allTypes.AddRange(assembly.GetTypes());
                }
            }
            //反射框架入口
            Type entryType = allTypes.First(type => type.FullName == "ZFramework.Game");
            IGameInstance game = Activator.CreateInstance(entryType, true) as IGameInstance;
            game.Start(allTypes.ToArray());
            return game;
        }

        void Update() => game.Update();
        void LateUpdate() => game.LateUpdate();
        void OnApplicationQuit() => game.Close();
    }
}