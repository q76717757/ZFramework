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
    public class BootProfile
    {
        [SerializeField] private bool isEnableHotfixCode = false;
        [SerializeField] private string[] hotfixAssemblyNames = new string[]
        {
            "Unity.Core",
            "Unity.Data",
            "Unity.Func",
            "Unity.View",
            "Assembly-CSharp",
        };
        [SerializeField] private string[] aotMetaAssemblyNames = new string[]
        {
            "mscorlib",
            "System",
            "System.Core",
            "UnityEngine.CoreModule",
            "Unity.Package.Runtime",
        };


        /// <summary>
        /// 是否启用代码热更新
        /// </summary>
        public bool IsEnableHotfixCore { get => isEnableHotfixCode; }
        /// <summary>
        /// 热更新程序集
        /// </summary>
        public string[] HotfixAssemblyNames { get => hotfixAssemblyNames; }
        /// <summary>
        /// 补充元数据程序集
        /// </summary>
        public string[] AotMetaAssemblyNames { get => aotMetaAssemblyNames; }


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




        static BootProfile _instance;
        bool isDirty;

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
            return Path.Combine(Defines.ProfilesAPath, "BootProfile.json");
        }
        internal void Save()
        {
            Directory.CreateDirectory(Defines.ProfilesAPath);
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

        internal void SetEnableHotfix(bool enable)
        {
            if (isEnableHotfixCode != enable)
            {
                isEnableHotfixCode = enable;
                SetDirty();
            }
        }
    }
}
