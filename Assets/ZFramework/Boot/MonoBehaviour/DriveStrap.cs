using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    [AddComponentMenu("")]
    public class DriveStrap : MonoBehaviour
    {
        IEntry entry;
        public void StartGame(Assembly assembly)
        {
            entry = assembly.GetType("ZFramework.Game").GetProperty("GameLoop").GetValue(null) as IEntry;
            entry.Load(assembly);
        }
        public void CloseGame()
        {
            if (entry != null)
            {
                entry.Close();
                Destroy(this);
            }
        }

        private void Update() => entry.Update();
        private void LateUpdate() => entry.LateUpdate();
    }

}
