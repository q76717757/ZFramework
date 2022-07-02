using UnityEngine;
using System;

namespace ZFramework
{
    public enum RunMode
    {
        HotReload_EditorRuntime, //热重载模式
        Mono_RealMachine,//发布模式
    }

    [DisallowMultipleComponent]
    public class BootStrap : MonoBehaviour
    {
        public IEntry entry;
        public RunMode mode;

        private void Awake()
        {
            Define.RunMode = mode;
            DontDestroyOnLoad(gameObject);
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Debug.Log(e.ExceptionObject.ToString());
            };
        }

        public void Start()
        {
            entry = AssemblyLoader.GetEntry(mode);
            if (entry != null)
            {
                entry.Start();
            }
            else
            {
                Debug.Log("Entry is Null");
                Destroy(this);
            }
        }
        private void Update() => entry.Update();
        private void LateUpdate() => entry.LateUpdate();
        private void OnApplicationQuit() => entry.Close();

    }

}