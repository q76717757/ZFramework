using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

namespace ZFramework
{
    internal class DownLoadComponent : MonoBehaviour
    {
        List<UnityWebRequest> loadRequest = new List<UnityWebRequest>();//这个用于存储所有的网络正在下载的资源文件，当退出时，在OnDestroy释放网络资源;
        private static DownLoadComponent instance;
        public static DownLoadComponent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<DownLoadComponent>();
                    if (instance == null)
                    {
                        instance = new GameObject().AddComponent<DownLoadComponent>();
                    }
                }
                return instance;
            }
        }
        //文件名字按照PPT名称+页数,先检查服务器是否存在这个文件,如果存在的话就不需要上传。
        private IEnumerator UploadFile(List<byte[]> fileData, Action<bool, List<string>> onComplete)
        {
            string host = "";// $"http://{Game.InnerHost}:7887/upload";
            List<string> paths = new List<string>();
            int count = 3;
            long currentTime = System.DateTime.Now.Ticks;
            for (int i = 0; i < fileData.Count; i++)
            {
                UnityWebRequest request = new UnityWebRequest(host, UnityWebRequest.kHttpVerbPOST);
                request.uploadHandler = new UploadHandlerRaw(fileData[i]);
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("filepath", "/YiXueJiaoXueGuan-PPTPDF/Texture-" + currentTime + $"/{i}.jpg");//想存在服务器的位置

                var ao = request.SendWebRequest();
                loadRequest.Add(request);
                while (true)
                {
                    if (!ao.isDone)
                    {
                        yield return null;
                    }
                    else
                    {
                        if (ao.webRequest.error != null)
                        {
                            Debug.Log(ao.webRequest.error);
                            if (count == 0)
                            {
                                onComplete?.Invoke(false, null);
                                loadRequest.Remove(request);
                                Log.Info("失败");
                                yield break;
                            }
                            else
                            {
                                i--;
                                count--;
                                Log.Info("重试");
                            }
                            break;
                            //返回失败/重试?
                        }
                        else
                        {
                            //上传成功
                            paths.Add(ao.webRequest.downloadHandler.text);
                            loadRequest.Remove(request);
                        }

                        break;
                    }
                }
            }
            onComplete?.Invoke(true, paths);
        }

        private IEnumerator DownloadFile(List<string> filePath, Action<bool, List<byte[]>> onComplete)
        {
            List<byte[]> spritesData = new List<byte[]>();
            int count = 3;
            for (int i = 0; i < filePath.Count; i++)
            {
                string host = "";// $"http://{Game.InnerHost}:7887/download?file=" + filePath[i];

                UnityWebRequest request = new UnityWebRequest(host, UnityWebRequest.kHttpVerbGET);
                request.downloadHandler = new DownloadHandlerBuffer();

                var ao = request.SendWebRequest();
                loadRequest.Add(request);
                while (true)
                {
                    if (!ao.isDone)
                    {
                        yield return null;
                    }
                    else
                    {
                        if (ao.webRequest.error != null)
                        {
                            if (count == 0)
                            {
                                onComplete?.Invoke(false, null);
                                //下载失败
                                Debug.Log(ao.webRequest.downloadHandler.text);//错误信息
                                loadRequest.Remove(request);
                                yield break;
                            }
                            else
                            {
                                i--;
                                count--;
                            }
                            //下载失败
                            Debug.Log(ao.webRequest.downloadHandler.text);//错误信息
                        }
                        else
                        {
                            //下载成功
                            byte[] texData = ao.webRequest.downloadHandler.data;//图片数据
                            spritesData.Add(texData);
                            loadRequest.Remove(request);
                        }
                        break;
                    }
                }
            }
            onComplete?.Invoke(true, spritesData);

        }
        public void StartUploadFile(List<byte[]> fileByte, Action<bool, List<string>> onComplete)
        {
            StartCoroutine(UploadFile(fileByte, onComplete));
        }


        public void StartDownloadFile(List<string> path, Action<bool, List<byte[]>> onComplete)
        {
            StartCoroutine(DownloadFile(path, onComplete));
        }
        private void OnDestroy()
        {
            StopCoroutine("UploadFile");
            StopCoroutine("DownloadFile");
            Log.Info("退出场景，当前正在下载的资源的数量是:" + loadRequest.Count);
            for (int i = 0; i < loadRequest.Count; i++)
            {
                if (loadRequest[i] != null)
                {
                    loadRequest[i].Dispose();
                }
            }
        }
    }
}
