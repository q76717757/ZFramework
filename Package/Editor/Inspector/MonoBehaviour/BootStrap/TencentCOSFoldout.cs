using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;
using System.Linq;
using System.Xml;
using System.IO;

namespace ZFramework.Editor
{
    public class TencentCOSFoldout : FadeFoldout
    {
        public override string Title => "对象存储(腾讯云)";

        string APPID = "1300651736";
        string secretId = "AKIDDZptthdNvMceR45wZ2JRYhrxudDalkmu";
        string secretKey = "YEqRq7jmfMwRRGE6wlYM6cfipw352teK";

        string region = "ap-guangzhou";
        string bucket = $"daschow";

        TencentCOS cos;
        TencentCOS COS
        {
            get
            {
                if (cos == null)
                {
                    cos = new TencentCOS(APPID, secretId, secretKey);
                    cos.Region= region;
                    cos.Bucket= bucket;
                }
                return cos;
            }
        }

        //http请求部分目前使用的是unitywebrequest 后续换成自己实现的http模块  让服务端也可以调用腾讯云的对象存储
        protected override void OnGUI()
        {
            COS.APPID = TextArea("APPID:", COS.APPID);
            COS.Region = TextArea("Region:", COS.Region);
            COS.SecretId = TextArea("SecretId:", COS.SecretId);
            COS.SecretKey = TextArea("SecretKey:", COS.SecretKey);
            COS.Bucket = TextArea("Bucket:", COS.Bucket);

            if (GUILayout.Button("存储桶是否存在，是否有权限访问"))
            {
                COS.HEADBucket().Invoke();
            }
            if (GUILayout.Button("列出该存储桶内的部分或者全部对象"))
            {
                COS.GETBucket().Invoke();
            }
            if (GUILayout.Button("将本地的对象（Object）上传至指定存储桶中"))
            {
                COS.PUTObject("temp.txt", Encoding.UTF8.GetBytes("测试上传一个txt文件")).Invoke();
            }
            if (GUILayout.Button("判断指定对象是否存在和有权限"))
            {
                COS.HEADObject("temp.txt").Invoke();
            }
            if (GUILayout.Button("将 COS 存储桶中的对象（Object）下载至本地"))
            {
                COS.GETObject("temp.txt").Invoke();
            }
            if (GUILayout.Button("分块下载"))
            {
                COS.GETObjectRange("temp.txt", "0", "14").Invoke();
            }
            if (GUILayout.Button("删除一个指定的对象（Object）"))
            {
                COS.DELETEObject("temp.txt").Invoke();
            }
            if (GUILayout.Button("一次性删除多个对象"))
            {
                COS.DELETEMultipleObjects(new string[]
                {
                    "iOS/",//文件夹以'/'结尾
                    "temp.txt",
                    "temp2.txt",
                }).Invoke();
            }

            EditorGUILayout.Space(10);
            uploadID = TextArea("UploadID", uploadID);
            etag = TextArea("ETag", etag);
            abName = TextArea("FilePath", abName);
            EditorGUILayout.LabelField("分块上传", EditorStyles.boldLabel);
            if (GUILayout.Button("初始化"))
            {
                COS.MultipartUpload_Initiate("test.ab").Invoke();
            }
            if (GUILayout.Button("分块上传到 COS"))
            {
                COS.UploadPart("test.ab", uploadID, 1, Encoding.UTF8.GetBytes("测试分块上传,就传一块试一下")).Invoke();
            }
            if (GUILayout.Button("完成分块"))
            {
                COS.CompleteMultipartUpload("test.ab", uploadID, new Dictionary<int, string>()
                {
                    [1] = etag
                }).Invoke();
            }
            if (GUILayout.Button("查询正在进行中的分块上传任务"))
            {
                COS.ListMultipartUploads().Invoke();
            }
            if (GUILayout.Button("舍弃一个分块上传并删除已上传的块"))
            {
                COS.AbortMultipartUpload(abName, uploadID).Invoke();
            }
            if (GUILayout.Button("查询特定分块上传中的已上传的块"))
            {
                COS.ListParts("test.ab", uploadID).Invoke();
            }
        }

        string uploadID;
        string etag;
        string abName;
        string TextArea(string label , string input)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(80));
            input = EditorGUILayout.TextField(input);
            EditorGUILayout.EndHorizontal();
            return input;
        }

    }
}
