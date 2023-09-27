using System;
using UnityEngine;
using UnityEngine.Networking;

namespace ZFramework
{
    public static class UnityWebRequestUtility
    {
        public static DownloadHandler DownLoad(string uriString)
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
