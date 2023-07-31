using UnityEngine;

namespace ZFramework
{
    [AddComponentMenu("")]
    internal class UnityLinker : MonoBehaviour
    {
        IGameInstance game;
        public void StartGame(IGameInstance game) => this.game = game;
        private void Start() => game.Start();
        private void Update() => game.Update();
        private void LateUpdate() => game.LateUpdate();
        private void OnApplicationQuit() => game.Close();
    }

}
