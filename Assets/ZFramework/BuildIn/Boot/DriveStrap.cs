using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    public interface IEntry
    {
        void Load(Assembly code);
        void Load(Assembly model, Assembly logic);
        void Reload(Assembly logic);

        void Update();
        void LateUpdate();
        void Close();
    }

    [AddComponentMenu("")]
    public class DriveStrap : MonoBehaviour
    {
        IEntry entry;
        public void Init(IEntry entry)=> this.entry = entry;
        private void Update()=> entry.Update();
        private void LateUpdate() => entry.LateUpdate();
        private void OnApplicationQuit() => entry.Close();
    }
}
