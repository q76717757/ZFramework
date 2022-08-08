using UnityEngine;
using System.Reflection;
using System;
using System.Collections;

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

    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public class BootStrap : MonoBehaviour
    {
        public BootFile boot;
        public IEntry entry;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ET.ETTask.ExceptionHandler += (e) =>
            {
                Debug.Log(e.Message);
            };
        }

        async void Start() => entry = await AssemblyLoader.GetEntry(boot);
        void Update() => entry?.Update();
        void LateUpdate()=> entry?.LateUpdate();
        void OnApplicationQuit() => entry?.Close();
    }
}
