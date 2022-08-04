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

    [CreateAssetMenu(fileName = "BootFile", menuName = "ZFramework/引导文件", order = 0)]
    public class BootFile : ScriptableObject
    {
        public string projectName;
        public AssemblyLoadPathType assemblyLoadPathType;

    }
}
