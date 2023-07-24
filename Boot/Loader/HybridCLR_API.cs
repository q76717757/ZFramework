using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ZFramework
{
    public static class HybridCLR_API
    {
        public static void LoadMetadataForAOTAssembly(string[] aotAssemblyPath)
        {
#if ENABLE_HYBRIDCLR
            foreach (var aotName in aotAssemblyPath)
            {
                UnityWebRequest www = UnityWebRequest.Get($"file://{Defines.AOTMetaAssemblyAPath}/{aotName}.dll.bytes");
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
    }
}
