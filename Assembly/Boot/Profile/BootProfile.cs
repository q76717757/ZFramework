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
        /// 配置文件是否存在
        /// </summary>
        public static bool IsExists
        {
            get
            {
                return File.Exists(GetFilePath());
            }
        }


        const string FILE_NAME = "BootProfile.json";
        static BootProfile _instance;
        bool isDirty = true;

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
            UnityWebRequest webRequest = null;
            try
            {
                webRequest = new UnityWebRequest(new Uri(GetFilePath()));
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.disposeDownloadHandlerOnDispose = true;
                webRequest.SendWebRequest();

                while (!webRequest.isDone) { }

                if (webRequest.error == null)
                {
                    string json = webRequest.downloadHandler.text;
                    _instance = JsonUtility.FromJson<BootProfile>(json);
                }
                else
                {
                    Log.Error("BootProfile读取失败:" + webRequest.error);
                }
            }
            catch (Exception e)
            {
                Log.Error("BootProfile读取失败:" + e);
            }
            finally
            {
                webRequest?.Dispose();
            }
            return _instance;
        }
        internal static string GetFilePath()
        {
            return Path.Combine(Defines.BuildInAssetAPath, "Profiles", FILE_NAME);
        }
        internal void Save()
        {
            Directory.CreateDirectory(Path.Combine(Defines.BuildInAssetAPath, "Profiles"));
            string json = JsonUtility.ToJson(this, true);
            File.WriteAllText(GetFilePath(), json);
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
