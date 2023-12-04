using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZFramework
{
    /// <summary>
    /// 基于腾讯云对象存储的文件服务实现
    /// </summary>
    public class CosFileServer : IFileServer
    {
        private TencentCOS cosServer;
        public CosFileServer()
        {
            cosServer = new TencentCOS()
            {
                APPID = "1300651736",
                SecretId = "AKIDDZptthdNvMceR45wZ2JRYhrxudDalkmu",
                SecretKey = "YEqRq7jmfMwRRGE6wlYM6cfipw352teK",
                Region = "ap-guangzhou",
                Bucket = "daschow",
            };
        }

        //查询
        public async ATask<bool> GetAllFiles()
        {
            return await cosServer.HEADBucket();
        }
        public async ATask<(bool,int)> Exists(string filePath)
        {
            return await cosServer.HEADObject(filePath);
        }

        //上传
        public async ATask<bool> UploadFile(string filePath, byte[] uploadBytes)
        {
            return await cosServer.PUTObject(filePath, uploadBytes);
        }
        //流式上传
        public async ATask UploadFile(string filePath, Stream stream)
        {
            (bool, string) a = await cosServer.MultipartUpload_Initiate(filePath);
            byte[] buffer = new byte[1024 * 1024 * 1];//一次传1M
            if (a.Item1)
            {
                Dictionary<int, string> eTags = new Dictionary<int, string>();
                string uploadID = a.Item2;
                int index = 1;
                while (true)
                {
                    int size = stream.Read(buffer, 0, buffer.Length);
                    if (size > 0)
                    {
                        byte[] send = new byte[size];
                        Array.Copy(buffer, 0, send, 0, size);
                        (bool, string) b = await cosServer.UploadPart(filePath, uploadID, index, send);
                        if (b.Item1)
                        {
                            string eTag = b.Item2;
                            eTags.Add(index, eTag);
                            index++;
                        }
                        else
                        {
                            Log.Error(b.Item2);
                            return;
                        }
                    }
                    else//上传完成
                    {
                        bool c =  await cosServer.CompleteMultipartUpload(filePath, uploadID, eTags);
                        if (c)
                        {
                            Log.Info("分块上传完成");
                        }
                        return;
                    }
                }
            }
            else
            {
                Log.Error("分块上传初始化失败->" + a.Item2);
            }
        }

        //下载
        public async ATask<byte[]> DownloadFile(string filePath)
        {
            return await cosServer.GETObject(filePath);
        }
        //流式下载
        public async ATask DownloadFileRange(string filePath, Stream stream)
        {
            (bool, int) a = await Exists(filePath);
            if (a.Item1)
            {
                int size = a.Item2;
                int index = 0;
                int bufferLenght = 1024 * 1024;//每块1M下载
                while (true)
                {
                    if (index + bufferLenght < size)
                    {
                        byte[] bytes = await cosServer.GETObjectRange(filePath, index.ToString(), (index + bufferLenght).ToString());
                        stream.Write(bytes, 0, bytes.Length);
                        index += bufferLenght;
                    }
                    else//最后一块
                    {
                        byte[] bytes = await cosServer.GETObjectRange(filePath, index.ToString(), "");
                        stream.Write(bytes, 0, bytes.Length);
                        Log.Info("download succses");
                        return;
                    }
                }
            }
            else
            {

            }
        }

        //删除
        public ATask<bool> DeleteFile(string filePath)
        {
            return cosServer.DELETEObject(filePath);
        }
        public ATask<bool> DeleteFiles(string[] filePaths)
        {
            return cosServer.DELETEMultipleObjects(filePaths);
        }
    }
}
