using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace ZFramework
{
    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public sealed class BootStrap : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(gameObject);

            Log.ILog = new UnityLogger();
            ZLogVisualization.Visua = true;


            gameObject.AddComponent<AssemblyLoader>().Load(OnCompletion);
        }

        void OnCompletion(List<Type> allTypes)
        {
            var game = allTypes.First(type => type.FullName == "ZFramework.Game")
                .GetMethod("CreateInstance", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, Array.Empty<object>()) as IGameInstance;
            game.Init(allTypes.ToArray());

            gameObject.AddComponent<TractionEngine>().StartGame(game);
        }
    }

}