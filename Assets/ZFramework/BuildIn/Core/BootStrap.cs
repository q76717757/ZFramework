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
    public enum CompileMode//编辑器下选择开发模式  使用热重载模式  发布后是无效枚举
    {
        Development,//开发模式
        Release,//发布模式
    }

    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public class BootStrap : MonoBehaviour
    {
        public IEntry entry;
        public CompileMode mode;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            ET.ETTask.ExceptionHandler += (e) =>
            {
                Debug.Log(e.Message);
            };
        }

        void Start() => entry = AssemblyLoader.GetEntry(mode);
        void Update() => entry.Update();
        void LateUpdate() => entry.LateUpdate();
        void OnApplicationQuit() => entry.Close();

    }

}
