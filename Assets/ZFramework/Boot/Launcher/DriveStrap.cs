using UnityEngine;

namespace ZFramework
{
    [AddComponentMenu("")]
    public class DriveStrap : MonoBehaviour
    {
        IEntry entry;
        public void StartGame(BootFile boot)
        {
            var codesAssembly = AssemblyLoader.LoadCode();
            entry = codesAssembly.GetType("ZFramework.Game").GetProperty("GameLoop").GetValue(null) as IEntry;
            entry.Load(codesAssembly);
        }
        public void CloseGame()
        {
            entry.Close();
        }

        private void Update() => entry.Update();
        private void LateUpdate() => entry.LateUpdate();
    }
}
