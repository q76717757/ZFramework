using System.Reflection;
using System;
using UnityEngine;

namespace ZFramework
{
    [AddComponentMenu("")]
    internal class TractionEngine : MonoBehaviour
    {
        IGameInstance game;

        public void StartGame(IGameInstance game)
        {
            hideFlags = HideFlags.HideInHierarchy;
            this.game = game;

        }

        private void Update() => game.Update();
        private void LateUpdate() => game.LateUpdate();
        private void OnApplicationQuit() => game.Close();
    }

}
