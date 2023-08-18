using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ZFramework
{
    public class WebRequestComponent :Component
    {
        public void Awake()
        {
            UnityWebRequest webRequest = new UnityWebRequest("https://api.jiugemeta.com/api/system/third/common/send_code/v2", UnityWebRequest.kHttpVerbPOST); //webRequest

            var json = new JsonData()
            {
                ["phone"] = 17688824224
            }.ToJson();

            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            webRequest.SetRequestHeader("Content-Type", "application/json");

            webRequest.SetRequestHeader("client-type", "yxn-phone");
            webRequest.SetRequestHeader("yxn-appid", "gsim-test-1663234906");
            webRequest.SetRequestHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJsb2dpbklkIjoiYXBwX3VzZXI6MTMxNzQ2Iiwicm4iOiIxNFhucVp1d3lHTmZCTmFoWUNHSE8zTjhEWkVBUDk5TSJ9.Gl9QuBrF-b7_HecYzcsqAvHnCTemNdsn1Pvc9npIWzY");
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            //await webRequest.SendWebRequest();
        }

        //public async AsyncTask<string> SendGet(string host)
        //{
        //    UnityWebRequest webRequest = new UnityWebRequest(host, UnityWebRequest.kHttpVerbGET);
        //    webRequest.downloadHandler = new DownloadHandlerBuffer();
        //    await webRequest.SendWebRequest();
        //    return webRequest.downloadHandler.text;
        //}

        //public async AsyncTask<string> SendPost(string host,string json)
        //{
        //    UnityWebRequest webRequest = new UnityWebRequest(host, UnityWebRequest.kHttpVerbPOST);
        //    webRequest.downloadHandler = new DownloadHandlerBuffer();
        //    webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        //    webRequest.SetRequestHeader("Content-Type", "application/json");

        //    webRequest.SetRequestHeader("client-type", "yxn-phone");
        //    webRequest.SetRequestHeader("yxn-appid", "gsim-test-1663234906");
        //    webRequest.SetRequestHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJsb2dpbklkIjoiYXBwX3VzZXI6MTMxNzQ2Iiwicm4iOiIxNFhucVp1d3lHTmZCTmFoWUNHSE8zTjhEWkVBUDk5TSJ9.Gl9QuBrF-b7_HecYzcsqAvHnCTemNdsn1Pvc9npIWzY");

        //    await webRequest.SendWebRequest();
        //    return webRequest.downloadHandler.text;
        //}

        //public async AsyncTask<string> 获取验证码(string phoneNum)
        //{
        //    var json = new JsonData()
        //    {
        //        ["phone"] = phoneNum
        //    }.ToJson();
        //    return await SendPost("https://api.jiugemeta.com/api/system/third/common/send_code/v2", json);
        //}

        //public async AsyncTask<string> 登录(string phoneNum, string code)
        //{
        //    var json = new JsonData()
        //    {
        //        //["code"] = "",
        //        //["encryptedData"] = "",
        //        //["iv"] = "",
        //        //["loginVersion"] = "",
        //        //["password"] = "",
        //        ["phone"] = phoneNum,
        //        ["phoneCode"] = code,
        //    }.ToJson();
        //    return await SendPost("https://api.jiugemeta.com/api/auth/third/login", json);
        //}

    }
}
