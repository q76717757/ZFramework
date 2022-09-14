using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ZFramework
{
    public static class HttpSystem
    {
        //public static async Task<HttpCallback> SendGetAsync(this HttpComponent component, string url)
        //{
        //    using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET))
        //    {
        //        request.downloadHandler = new DownloadHandlerBuffer();
        //        await request.SendWebRequest();

        //        if (request.result != UnityWebRequest.Result.Success)
        //        {
        //            //Log.Info($"SendGetCallback->{request.error}");
        //            return new HttpCallback()
        //            {
        //                success = false,
        //                msg = request.error
        //            };
        //        }
        //        else
        //        {
        //            //Log.Info($"SendGetCallback->{request.downloadHandler.text}");
        //            return new HttpCallback()
        //            {
        //                success = true,
        //                msg = request.downloadHandler.text,
        //            };
        //        }
        //    }
        //}
        //public static async Task<HttpCallback> SendPostAsync<T>(this HttpComponent component, string url, string json)
        //{
        //    using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        //    {
        //        //Log.Info($"SendPost->{url}:{json}");
        //        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        //        request.SetRequestHeader("Content-Type", "application/json");
        //        request.downloadHandler = new DownloadHandlerBuffer();
        //        await request.SendWebRequest();
        //        if (request.result != UnityWebRequest.Result.Success)
        //        {
        //            //Log.Info($"SendPostCallback->{url}:{request.error}");
        //            return new HttpCallback()
        //            {
        //                success = false,
        //                msg = request.error
        //            };
        //        }
        //        else
        //        {
        //            //Log.Info($"SendPostCallback->{url}:{request.downloadHandler.text}");
        //            return new HttpCallback()
        //            {
        //                success = true,
        //                msg = request.downloadHandler.text,
        //                data = request.downloadHandler.data
        //            };
        //        }
        //    }
        //}

        //public static async Task<HttpCallback> UPloadFileAsync(this HttpComponent component, string url, string filename, string folderName, byte[] bytes)
        //{
        //    //Log.Info($"Upload-->{folderName}/{filename}:{bytes.Length}");
        //    WWWForm form = new WWWForm();
        //    form.AddField("Content-Type", "application/json");
        //    form.AddField("charset", "UTF-8");
        //    form.AddField("foldername", folderName);
        //    form.AddBinaryData("file", bytes, filename);
        //    using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        //    {
        //        await request.SendWebRequest();
        //        if (request.result != UnityWebRequest.Result.Success)
        //        {
        //            //Log.Info($"UploadCallback->{filename}:{webRequest.error}");
        //            return new HttpCallback()
        //            {
        //                success = false,
        //                msg = request.error,
        //            };
        //        }
        //        else
        //        {
        //            string returnStr = request.downloadHandler.text;
        //            //Log.Info($"UploadCallback->{filename}:{returnStr}");
        //            return new HttpCallback()
        //            {
        //                success = false,
        //                msg = request.downloadHandler.text,
        //            };
        //        }
        //    }
        //}
        //public static async Task<HttpCallback> DownloadFileAsync(this HttpComponent component, string downloadUrl)
        //{
        //    //Log.Info("Download-->" + downloadUrl);
        //    using (UnityWebRequest request = UnityWebRequest.Get(downloadUrl))
        //    {
        //        request.downloadHandler = new DownloadHandlerBuffer();
        //        await request.SendWebRequest();
        //        if (request.result != UnityWebRequest.Result.Success)
        //        {
        //            //Log.Info($"DownloadCallback->{downloadUrl}:{webRequest.error}");
        //            return new HttpCallback()
        //            {
        //                success = false,
        //                msg = request.error,
        //            };
        //        }
        //        else
        //        {
        //            //Log.Info($"DownloadCallback->{downloadUrl}:{webRequest.downloadHandler.data.Length}");
        //            return new HttpCallback()
        //            {
        //                success = true,
        //                data = request.downloadHandler.data,
        //            };
        //        }
        //    }
        //}

        //public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
        //{
        //    var tcs = new TaskCompletionSource<object>();
        //    asyncOp.completed += obj => { tcs.SetResult(null); };
        //    return ((Task)tcs.Task).GetAwaiter();
        //}

    }
}
