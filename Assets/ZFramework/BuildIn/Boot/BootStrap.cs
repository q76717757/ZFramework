using System;
using UnityEngine;

namespace ZFramework
{
    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public class BootStrap : MonoBehaviour
    {
        public bool codeMod;
        public BootFile boot;

        void Start()
        {
            Load(boot);
            Destroy(gameObject);
        }

        public static async void Load(BootFile boot)
        {
            var entry = await AssemblyLoader.GetEntry(boot);
            new GameObject("[ZFramework]").AddComponent<DriveStrap>().Init(entry);
        }
    }
}
