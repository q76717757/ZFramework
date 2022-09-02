using UnityEngine;

namespace ZFramework
{
    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public class BootStrap : MonoBehaviour
    {
        public BootFile boot;

        async void Start()
        {
            var entry = await AssemblyLoader.GetEntry(boot);
            if (entry != null)
            {
                gameObject.AddComponent<DriveStrap>().Init(entry);
            }
            DontDestroyOnLoad(gameObject);
        }
    }
}
