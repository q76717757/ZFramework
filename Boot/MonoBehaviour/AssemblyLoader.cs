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
        internal static void LoadAssembly() => new GameObject() { hideFlags = HideFlags.HideInHierarchy }.AddComponent<AssemblyLoader>().Load();
        void Load()
        {
            switch (ZFrameworkRuntimeSettings.Get().AssemblyLoadType)
            {
                case Defines.UpdateType.Not:
                    LoadFromAppDomain();
                    break;
                case Defines.UpdateType.Online:
                    StartCoroutine(LoadFromOnline());
                    break;
                case Defines.UpdateType.Offline:
                    StartCoroutine(LoadFromOffline());
                    break;
            }
        }
        void LoadCompleted(List<Type> allTypes)
        {
            var game = allTypes.First(type => type.FullName == "ZFramework.Game")
                .GetMethod("CreateInstance", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, Array.Empty<object>()) as IGameInstance;
            game.Init(allTypes.ToArray());

            gameObject.AddComponent<TractionEngine>().StartGame(game);
            Destroy(this);
        }


        void LoadFromAppDomain()
        {
            string[] targetDlls = ZFrameworkRuntimeSettings.Get().AssemblyNames;
            var allTypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (targetDlls.Contains(assembly.GetName().Name))
                {
                    allTypes.AddRange(assembly.GetTypes());
                }
            }
            LoadCompleted(allTypes);
        }

        IEnumerator LoadFromOnline()
        {
            //AOT补充元数据
            yield return LoadMetadataForAOTAssembly();

            var settings = ZFrameworkRuntimeSettings.Get();
            if (settings.MustNew /*  || 自动更新  */)
            {
                //版本对比
                //---> 远程加载程序集
            }
            else
            {
                //本地加载程序集缓存
            }

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

            var jitPath = ZFrameworkRuntimeSettings.Get().AssemblyNames;
            //Load JIT
            List<Type> allTypes = new List<Type>();
            foreach (var item in jitPath)
            {
                byte[] dllBytes = default;// dllAssetBytes[item];
                var type = Assembly.Load(dllBytes).GetTypes();
                allTypes.AddRange(type);
            }
            LoadCompleted(allTypes);
        }

        IEnumerator LoadFromOffline()
        {
            //AOT补充元数据
            yield return LoadMetadataForAOTAssembly();
            //从本地加载JIT AB包
            UnityWebRequest www = UnityWebRequest.Get($"file://{Application.streamingAssetsPath}/JIT.unity3d");
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Log.Error($"JIT包读取失败:{www.error}");
                yield break;
            }
            //从AB包读取程序集
            var ab = AssetBundle.LoadFromMemory(www.downloadHandler.data);
            var allTypes = new List<Type>();
            foreach (var jitName in ZFrameworkRuntimeSettings.Get().AssemblyNames)
            {
                //加载程序集
                var dllBytes = ab.LoadAsset<TextAsset>($"{jitName}.dll.bytes").bytes;
                var type = Assembly.Load(dllBytes).GetTypes();
                allTypes.AddRange(type);
            }
            LoadCompleted(allTypes);
        }

        //远程下载JIT DLL
        IEnumerator DownloadJIT()
        {
            var jitPath = ZFrameworkRuntimeSettings.Get().AssemblyNames;
            yield return null;
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





        IEnumerator LoadMetadataForAOTAssembly()
        {
#if UNITY_EDITOR
            yield break;
#endif
            yield break;

#if ENABLE_HYBRIDCLR
            foreach (var aotName in ZFrameworkRuntimeSettings.Get().AotMetaAssemblyNames)
            {
                UnityWebRequest www = UnityWebRequest.Get($"file://{Defines.AOTMetaAssemblyLoadDir}/{aotName}.dll.bytes");
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Log.Error($"{aotName}读取失败:{www.error}");
                }
                else
                {
                    byte[] assetData = www.downloadHandler.data;
                    HybridCLR.LoadImageErrorCode error = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(assetData, HybridCLR.HomologousImageMode.Consistent);
                    if (error != HybridCLR.LoadImageErrorCode.OK)
                    {
                        Log.Error($"{aotName}元数据补充失败:{error}");
                    }
                    else
                    {
                        Log.Info("AOT补充元数据->" + aotName);
                    }
                }
            }
#endif
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
