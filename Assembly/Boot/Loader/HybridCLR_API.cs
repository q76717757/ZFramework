#if ENABLE_HYBRIDCLR

using UnityEngine.Networking;
using HybridCLR;
using System;
using System.IO;

namespace ZFramework
{
    internal static class HybridCLR_API
    {
        public static void LoadMetadataForAOTAssembly(string[] aotAssemblyNames, HomologousImageMode mode = HomologousImageMode.Consistent)
        {
            foreach (var aotName in aotAssemblyNames)
            {
                Uri uri = new Uri(Path.Combine(Defines.AOTMetaAssemblyAPath, $"{aotName}.dll.bytes"));
                UnityWebRequest www = UnityWebRequest.Get(uri);
                www.SendWebRequest();
                while (!www.isDone)
                {
                }
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Log.Error($"{aotName}读取失败:{www.error}");
                }
                else
                {
                    byte[] assetData = www.downloadHandler.data;
                    LoadImageErrorCode error = RuntimeApi.LoadMetadataForAOTAssembly(assetData, mode);
                    if (error != LoadImageErrorCode.OK)
                    {
                        Log.Error($"{aotName}元数据补充失败:{error}");
                    }
                    else
                    {
                        Log.Info("AOT补充元数据->" + aotName);
                    }
                }
            }
        }
    }
}

#endif