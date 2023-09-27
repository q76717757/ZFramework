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
        internal static IGameInstance game;

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
            BootProfile profile = BootProfile.GetInstance();

            //根据引导配置预加载程序集
            if (profile.isEnableHotfixCode)
            {
#if !UNITY_EDITOR && ENABLE_HYBRIDCLR
                HybridCLRUtility.LoadAssembly();
#endif
            }
            //遍历域中所有已加载的程序集,获取框架管理的目标程序集
            List<Type> allTypes = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (profile.assemblyNames.Contains(assembly.GetName().Name))
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