using UnityEngine;

namespace ZFramework
{
    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public sealed class BootStrap : MonoBehaviour
    {
        public static BootStrap instance;
        void Start()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            AssemblyLoader.LoadAssembly();
        }
    }

}