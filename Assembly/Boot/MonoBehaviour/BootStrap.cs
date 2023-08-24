using System;
using System.Linq;
using UnityEngine;

namespace ZFramework
{
    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public sealed class BootStrap : MonoBehaviour
    {
        //引导启动流程图
        //https://www.processon.com/view/link/64b9f2dda554064ccf306779

        static IGameInstance game;

        void Start()
        {
            if (game != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Error("UnhandledException:" + e.ExceptionObject);
            };

            BootProfile bootConfig = BootProfile.GetInstance();
            IAssemblyLoader assemblyLoader = GetAssemblyLoader(bootConfig.AssemblyLoadType);
            Type[] allTypes = assemblyLoader.LoadAssembly(Defines.AssemblyNames);
            game = StartGame(allTypes);
        }

        IAssemblyLoader GetAssemblyLoader(Defines.UpdateType type)
        {
#if UNITY_EDITOR
            return new DomainLoader();
#else
            switch (type)
            {
                case Defines.UpdateType.Not:
                    return new DomainLoader();
                case Defines.UpdateType.Online:
                    return new OnlineLoader();
                case Defines.UpdateType.Offline:
                    return new OfflineLoader();
                default:
                    throw new NotImplementedException();
            }
#endif
        }
        IGameInstance StartGame(Type[] allTypes)
        {
            Type entryType = allTypes.First(type => type.FullName == "ZFramework.Game");
            IGameInstance game = Activator.CreateInstance(entryType, true) as IGameInstance;
            game.Start(allTypes);
            return game;
        }

        void Update() => game.Update();
        void LateUpdate() => game.LateUpdate();
        void OnApplicationQuit() => game.Close();
    }
}