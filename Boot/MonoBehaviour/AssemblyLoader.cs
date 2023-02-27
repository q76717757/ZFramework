using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.Networking;
using System.Reflection;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace ZFramework
{
    [AddComponentMenu("")]
    internal class AssemblyLoader : MonoBehaviour
    {
        Dictionary<string, byte[]> dllAssetBytes = new Dictionary<string, byte[]>();
        Action<List<Type>> onCompletion;

        internal void Load(Action<List<Type>> onCompletion)
        {
            this.onCompletion = onCompletion;

            if (Defines.IsEditor)
            { 
                LoadNot();
            }
            else
            {
                switch (ZFrameworkRuntimeSettings.Get().AssemblyLoadType)
                {
                    case HotUpdateType.Online:
                        StartCoroutine(LoadOnline());
                        break;
                    case HotUpdateType.Offline:
                        StartCoroutine(LoadOffline());
                        break;
                    case HotUpdateType.Not:
                        LoadNot();
                        break;
                }
            }
        }

        IEnumerator LoadOnline()
        {
            //AOT补充
            yield return LoadAOTFromStreamAssets();
            //获取本地程序集版本
            var localVersion = GetLocalAssemblyVersion();
            //获取远程程序集版本
            var remoteVersion = GetRemoteAssemblyVersion();
            //对比版本
            if (remoteVersion > localVersion)//有更新
            {
                //下载程序集资源

                //修改本地程序集版本

                //持久化到本地
            }
            //解包本地程序集

            //补充元数据//写到AOT协程内
            //加载程序集//写到JIT协程内
           
            yield return DownloadJIT();
            Loading();
        }
        IEnumerator LoadOffline()
        {
            //AOT补充
            yield return LoadAOTFromStreamAssets();

            yield return DownloadJIT();


        }
        void LoadNot()
        {
            string[] targetDlls = ZFrameworkRuntimeSettings.Get().AssemblyNames;
            List<Type> types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (targetDlls.Contains(assembly.GetName().Name))
                {
                    types.AddRange(assembly.GetTypes());
                }
            }
            LoadComlplition(types);
        }
        void LoadComlplition(List<Type> allTypes)
        {
            onCompletion(allTypes);
            Destroy(this);
        }

        IEnumerator LoadAOTFromStreamAssets()
        {
            var aotPath = Defines.AOTMetaAssemblyLoadDir;
            var aotNames = ZFrameworkRuntimeSettings.Get().AotMetaAssemblyNames;
            foreach (var item in aotNames)
            {
                var aotFileDir = $"file://{aotPath}/{item}.dll.bytes";
                Log.Info("Load Aot From ->" + aotFileDir);
                yield return LoadAOTFiles(aotFileDir);
            }
        }
        IEnumerator LoadAOTFiles(string path)
        {
            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Log.Info(www.error + "-->" + path);
            }
            else
            {
                byte[] assetData = www.downloadHandler.data;
                HybridCLR.LoadImageErrorCode err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(assetData, HybridCLR.HomologousImageMode.Consistent);
                if (err != HybridCLR.LoadImageErrorCode.OK)
                {
                    Log.Error(err + "-->" + path);
                }
            }
        }

        //远程下载JIT DLL
        IEnumerator DownloadJIT()
        {
            var jitPath = ZFrameworkRuntimeSettings.Get().AssemblyNames;
            yield return null;
        }

        void Loading()
        {
            var jitPath = ZFrameworkRuntimeSettings.Get().AssemblyNames;
            //Load JIT
            List<Type> types = new List<Type>();
            foreach (var item in jitPath)
            {
                byte[] dllBytes = dllAssetBytes[item];
                var type = Assembly.Load(dllBytes).GetTypes();
                types.AddRange(type);
            }
            LoadComlplition(types);
        }



        VersionNum GetLocalAssemblyVersion()
        {
            //本地持久化版本只有一个 以手机端为准
            //先找持久化目录
            //没有再找流目录  并且复制到持久目录
            //读文件,打包时要把程序集版本号带进流目录
            return default;
        }
        VersionNum GetRemoteAssemblyVersion()
        {
            var targetPlatform = Defines.TargetRuntimePlatform;
            return default;
        }





        public static string FileMD5(string filePath)
        {
            byte[] retVal;
            using (FileStream file = new FileStream(filePath, FileMode.Open))
            {
                using (MD5 md5 = MD5.Create())
                {
                    retVal = md5.ComputeHash(file);
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in retVal)
            {
                stringBuilder.Append(b.ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    } 
}
