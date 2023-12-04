using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZFramework
{
    /// <summary>
    /// 文件服务接口  提供远程文件存储查询等服务
    /// </summary>
    public interface IFileServer
    {
        //查询
        public ATask<bool> GetAllFiles();
        public ATask<(bool, int)> Exists(string filePath);

        //上传
        public ATask<bool> UploadFile(string filePath, byte[] uploadBytes);
        public ATask UploadFile(string filePath, Stream readStream);

        //下载
        public ATask<byte[]> DownloadFile(string filePath);
        public ATask DownloadFileRange(string filePath, Stream writeStream);

        //删除
        public ATask<bool> DeleteFile(string filePath);
        public ATask<bool> DeleteFiles(string[] filePaths);
    }
}
