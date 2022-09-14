using UnityEngine;

namespace ZFramework
{
    [AddComponentMenu("")]
    public class DriveStrap : MonoBehaviour
    {
        public IEntry entry;

        private void Awake() => DontDestroyOnLoad(gameObject);
        public void Init(IEntry entry) => this.entry = entry;
        private void Update() => entry.Update();
        private void LateUpdate() => entry.LateUpdate();
        private void OnApplicationQuit() => entry.Close();
    }
}
