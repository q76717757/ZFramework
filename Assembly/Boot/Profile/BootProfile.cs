using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;

namespace ZFramework
{
    /// <summary>
    /// 启动配置文件
    /// </summary>
    internal class BootProfile
    {
        /// <summary>
        /// 是否启用代码热更新
        /// </summary>
        public bool isEnableHotfixCode = false;
        /// <summary>
        /// 加载程序集
        /// </summary>
        public List<string> assemblyNames = new List<string>
        {
            "Unity.Assembly.Core",
            "Unity.Assembly.Data",
            "Unity.Assembly.Func",
            "Unity.Assembly.View",
            "Assembly-CSharp",
        };
        /// <summary>
        /// 补充元数据程序集
        /// </summary>
        public List<string> aotMetaAssemblyNames = new List<string>
        {
        };
        /// <summary>
        /// 热更新程序集
        /// </summary>
        public List<string> hotfixAssemblyNames = new List<string>
        {
            "Unity.Assembly.Core",
            "Unity.Assembly.Data",
            "Unity.Assembly.Func",
            "Unity.Assembly.View",
            "Assembly-CSharp",
        };

        /// <summary>
        /// 文件是否存在
        /// </summary>
        internal static bool IsExists
        {
            get
            {
                return File.Exists(FilePath);
            }
        }
        /// <summary>
        /// 文件路径
        /// </summary>
        internal static string FilePath
        {
            get
            {
                return Path.Combine(Defines.BuildInAssetAPath, "Profiles", "BootProfile.json");
            }
        }


        private static BootProfile _instance;
        private bool isDirty = true;

        internal static BootProfile GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            if (!IsExists)
            {
                Log.Error("BootProfile文件不存在");
                return null;
            }
            using (DownloadHandler download = UnityWebRequestUtility.DownLoad(FilePath))
            {
                _instance = JsonUtility.FromJson<BootProfile>(download.text);
            }
            return _instance;
        }
        internal void Save()
        {
            _instance = this;
            Directory.CreateDirectory(new FileInfo(FilePath).Directory.FullName);
            string json = JsonUtility.ToJson(this, true);
            File.WriteAllText(FilePath, json);
        }
        internal void SaveIfDirty()
        {
            if (isDirty)
            {
                Save();
                isDirty = false;
            }
        }
        internal void SetDirty()
        { 
            isDirty = true;
        }
    }
}
