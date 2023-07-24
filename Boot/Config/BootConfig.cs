using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;

namespace ZFramework
{
    public class BootConfig
    {
        static string _filePath = Path.Combine(Defines.BuildInAssetAPath, "Config", "BootConfig.json");
        static BootConfig _instance;
        static bool _isDirty;

        [SerializeField] private Defines.UpdateType assemblyLoadType;
        [SerializeField] private string[] assemblyNames;
        [SerializeField] private string[] aotMetaAssemblyNames;
        [SerializeField] private string versionURL;
        [SerializeField] private bool isMustSync;
        [SerializeField] private string metaAssemblyBundleHash;

        public static BootConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Load();
                }
                return _instance;
            }
        }
        public static bool IsDirty
        {
            get
            {
                return _isDirty;
            }
        }
        public static bool IsExists
        {
            get
            {
                return File.Exists(_filePath);
            }
        }
        /// <summary>
        /// 热更新模式
        /// </summary>
        public Defines.UpdateType AssemblyLoadType { get => assemblyLoadType; }
        /// <summary>
        /// 热更新程序集
        /// </summary>
        public string[] AssemblyNames { get => assemblyNames; }
        /// <summary>
        /// 补充元数据程序集
        /// </summary>
        public string[] AotMetaAssemblyNames { get => aotMetaAssemblyNames; }
        /// <summary>
        /// 版本文件下载链接
        /// </summary>
        public string VersionURL { get => versionURL; }
        /// <summary>
        /// 是否强同步
        /// </summary>
        public bool IsMustSync { get => IsMustSync; }
        /// <summary>
        /// 补充元数据的AB包hash值,在加载meta之前先检查即将要加载的jit所以来的aot是不是一致的  可以考虑使用超集
        /// </summary>
        public string MetaAssemblyBundleHash { get => metaAssemblyBundleHash; }

        

        private BootConfig() { }

        static BootConfig Load()
        {
            if (!IsExists)
            {
                Log.Error("BootConfig文件不存在");
                return null;
            }
            UnityWebRequest webRequest = null;
            try
            {
                webRequest = new UnityWebRequest(new Uri(_filePath));
                webRequest.timeout = 1;
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.disposeDownloadHandlerOnDispose = true;
                webRequest.SendWebRequest();

                while (!webRequest.isDone)
                {
                }

                if (webRequest.error == null)
                {
                    string json = webRequest.downloadHandler.text;
                    return JsonUtility.FromJson<BootConfig>(json);
                }
                else
                {
                    Log.Error("BootConfig读取失败:" + webRequest.error);
                    return null;
                }
            }
            catch (Exception e)
            {
                Log.Error("BootConfig读取失败:" + e.Message);
                return null;
            }
            finally
            {
                webRequest?.Dispose();
            }
        }




#if UNITY_EDITOR
        internal static void Create()
        {
            new BootConfig().Save();
        }
        internal void Save()
        {
            var json = JsonUtility.ToJson(this, true);
            File.WriteAllText(_filePath, json);
        }
        internal void SaveIfDirty()
        {
            if (IsDirty)
            {
                Save();
            }
        }
#endif

    }
}
