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
        [SerializeField] private Defines.UpdateType assemblyLoadType = Defines.UpdateType.Not;
        [SerializeField] private string[] assemblyNames = Defines.DefaultAssemblyNames;
        [SerializeField] private string[] aotMetaAssemblyNames = Defines.DefaultAOTMetaAssemblyNames;
        [SerializeField] private bool isMustSync = false;



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
        /// 是否强同步
        /// </summary>
        public bool IsMustSync { get => IsMustSync; }

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
            return Path.Combine(Defines.BootProfileAPath, "BootProfile.json");
        }
        internal void Save()
        {
            Directory.CreateDirectory(Defines.BootProfileAPath);
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
