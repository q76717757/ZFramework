using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public enum AssemblyLoadPathType
    {
        BuildIn,
        StreamingAssets,
        Resources,
        Network
    }
    public enum AssetsPath
    {
        Resources,
        Streamming,
        Other,
        Web,
    }
    public enum Platform
    {
        Win,
        Andorid,
        IOS,
        WebGL,
        UWP
    }

    [CreateAssetMenu(fileName = "BootFile", menuName = "ZFramework/引导文件", order = 0)]
    public class BootFile : ScriptableObject
    {
        public Platform platform;


        //public string projectName;
        public int useHotfix;
        public int hotfixType;

    }
}
