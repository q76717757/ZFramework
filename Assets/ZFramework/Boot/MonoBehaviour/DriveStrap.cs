using System;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    [AddComponentMenu("")]
    public class DriveStrap : MonoBehaviour
    {
        IGameInstance game;
        public void StartGame(Assembly assembly)
        {
            game = assembly.GetType("ZFramework.Game")
                .GetMethod("CreateInstance", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, Array.Empty<object>()) as IGameInstance;
            game.Load(assembly);
        }
        public void CloseGame()
        {
            game.Close();
            Destroy(this);
        }

        private void Update() => game.Update();
        private void LateUpdate() => game.LateUpdate();
    }

}
