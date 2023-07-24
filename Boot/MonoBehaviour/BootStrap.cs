using System;
using System.Linq;
using UnityEngine;

namespace ZFramework
{
    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public sealed class BootStrap : MonoBehaviour
    {
        //流程图
        //https://www.processon.com/view/link/64b9f2dda554064ccf306779

        void Start()
        {
            DontDestroyOnLoad(gameObject);

            BootConfig bootConfig = BootConfig.Instance;
            IAssemblyLoader loader = GetAssemblyLoader(bootConfig.AssemblyLoadType);
            Type[] allTypes =  loader.LoadAssembly(bootConfig.AssemblyNames);
            IGameInstance game = GetGameInstance(allTypes);

            gameObject.AddComponent<TractionEngine>().StartGame(game);
        }

        IAssemblyLoader GetAssemblyLoader(Defines.UpdateType type)
        {
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
        }

        IGameInstance GetGameInstance(Type[] allTypes)
        {
            Type entryType = allTypes.First(type => type.FullName == "ZFramework.Game");
            IGameInstance game = Activator.CreateInstance(entryType, true) as IGameInstance;
            game.Init(allTypes);

            return game;
        }

    }
}