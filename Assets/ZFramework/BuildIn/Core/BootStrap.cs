using UnityEngine;
using System;
using System.Reflection;

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
    public enum DllLoadMode//个别不支持load.dll的平台或者放弃热更 如uwp?  在构建时把dll直接包含进工程内
    {
        BuildIn,
        InFile,
    }
    public enum AssetsMode//针对不同应用场景  ab包的位置从不同的位置读取  有的是本地有的是远程  有的读不了AB包
    {
        Resources,
        StreamingAssets,
        Bundle,
        Remote,
    }

    public enum Platform//发布平台
    { 
        Windows,
        Android,
        IOS,
        //UWP  //HoloLens
        //WebGL
    }

    [DisallowMultipleComponent]
    public class BootStrap : MonoBehaviour
    {
        public IEntry entry;
        public CompileMode mode;
        public AssetsMode assetMode;
        public Platform platform;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            entry = AssemblyLoader.GetEntry(mode);
            if (entry == null)
            {
                Destroy(this);
            }
        }

        void Update() => entry.Update(/*Time.deltaTime,Time.unscaledDeltaTime*/);
        void LateUpdate() => entry.LateUpdate();
        void OnApplicationQuit() => entry.Close();
    }

}