using UnityEngine;
using System.Reflection;
using System;
using System.Collections;

namespace ZFramework
{
    public interface IEntry
    {
        void Start(Assembly code);
        void Start(Assembly model, Assembly logic);
        void Reload(Assembly logic);

        void Update();
        void LateUpdate();
        void Close();
    }

    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public class BootStrap : MonoBehaviour
    {
        public IEntry entry;
        public BootFile boot;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ET.ETTask.ExceptionHandler += (e) =>
            {
                Debug.Log(e.Message);
            };
        }
        void Start()
        {
            entry = AssemblyLoader.GetEntry(boot);
        }

        void Update()
        {
            entry.Update();
        }
        void LateUpdate()
        {
            entry.Update();
        }

        void OnApplicationQuit()
        {
            if (entry != null)
            {
                entry.Close();
                entry = null;
            }
        }
    }

}
