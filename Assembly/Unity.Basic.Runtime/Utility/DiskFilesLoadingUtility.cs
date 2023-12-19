using System;
using UnityEngine;
using UnityEngine.Networking;

namespace ZFramework
{
    /// <summary>
    /// 磁盘文件加载实用程序,基于UnityWebRequest,以同步方式加载本地磁盘的文件,用来在引导阶段加载配置文件和程序集等小型文件
    /// </summary>
    public static class DiskFilesLoadingUtility
    {
        public static string DownLoadText(string uriString)
        {
            using DownloadHandler download = DownLoad(uriString);
            return download.text;
        }

        private static DownloadHandler DownLoad(string uriString)
        {
            UnityWebRequest request = UnityWebRequest.Get(new Uri(uriString));
            try
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                request.disposeDownloadHandlerOnDispose = false;
                request.SendWebRequest();
                while (!request.isDone)
                {
                }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"UnityWebRequest Download {request.result}-->{request.error}");
                }
                else
                {
                    return request.downloadHandler;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                request.Dispose();
            }
        }

    }
}
