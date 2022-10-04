using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public enum Platform
    {
        Win,
        Mac,
        Andorid,
        IOS,
        WebGL,
        UWP
    }

    [CreateAssetMenu(fileName = "BootFile", menuName = "ZFramework/引导文件", order = 0)]
    public class BootFile : ScriptableObject, ISerializationCallbackReceiver
    {
        //私有序列化字段
        [SerializeField] private string projectCode;
        [SerializeField] private string projectName;
        [SerializeField] private string projectVersion;
        private VersionInfo versionInfo;

        public Platform platform;//目标平台
        public bool allowOffline;//允许离线

        public string buildInPath;
        public string cachePath;
        public string downloadPath;


        //公开属性
        public string ProjectCode => projectCode;
        public string ProjectName=> projectName;
        public VersionInfo Version => versionInfo;
        public string GetDllName() => $"{ProjectCode}_{Version.ToNumString()}";

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!VersionInfo.TryParse(projectVersion, out versionInfo))
            {
                projectVersion = Version.ToString();
            }
        }
    }
}
