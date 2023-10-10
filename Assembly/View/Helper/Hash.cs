using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

public class Hash : MonoBehaviour
{
    private const int BlockSize = 32768; // 每个数据块的大小 32kb

    private async void Start()
    {
        string folderPath = "path/to/large/file";  // 修改成自己的文件路径

        Dictionary<string, Dictionary<string, string>> fileHashes = await CalculateFolderFileHashesAsync(folderPath);

        foreach (var kvp in fileHashes)
        {
            string filePath = kvp.Key;
            Dictionary<string, string> hashValues = kvp.Value;

            Debug.Log("文件路径: " + filePath);
            Debug.Log("MD5 哈希值: " + hashValues["md5"]);
            Debug.Log("SHA-1 哈希值: " + hashValues["sha1"]);
        }

        string folderHash = CalculateFolderHash(fileHashes);

        Debug.Log("文件夹哈希值: " + folderHash);
    }


    //// 计算 MD5 哈希值
    //public string CalculateMD5Hash(string filePath)
    //{
    //    using var md5 = MD5.Create();
    //    using FileStream fs = File.OpenRead(filePath);
    //    byte[] buffer = new byte[BlockSize];
    //    int bytesRead = 0;

    //    while ((bytesRead = fs.Read(buffer, 0, BlockSize)) > 0)
    //    {
    //        md5.TransformBlock(buffer, 0, bytesRead, null, 0);
    //    }

    //    md5.TransformFinalBlock(new byte[0], 0, 0);

    //    return BytesToHexString(md5.Hash);
    //}

    //// 计算 SHA-1 哈希值
    //public string CalculateSHA1Hash(string filePath)
    //{
    //    using var sha1 = SHA1.Create();
    //    using FileStream fs = File.OpenRead(filePath);
    //    byte[] buffer = new byte[BlockSize];
    //    int bytesRead = 0;

    //    while ((bytesRead = fs.Read(buffer, 0, BlockSize)) > 0)
    //    {
    //        sha1.TransformBlock(buffer, 0, bytesRead, null, 0);
    //    }

    //    sha1.TransformFinalBlock(new byte[0], 0, 0);

    //    return BytesToHexString(sha1.Hash);
    //}


    /// <summary>
    /// 异步计算 MD5 哈希值
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public Task<string> CalculateMD5HashAsync(string filePath)
    {
        using var md5 = MD5.Create(); // 创建 MD5 实例
        using FileStream fs = File.OpenRead(filePath); // 打开文件流读取文件内容
        byte[] buffer = new byte[BlockSize]; // 创建缓冲区用于存储读取的数据块
        int bytesRead = 0;

        while ((bytesRead = fs.Read(buffer, 0, BlockSize)) > 0) // 以块为单位读取文件内容
        {
            md5.TransformBlock(buffer, 0, bytesRead, null, 0); // 更新 MD5 计算的中间结果
        }

        md5.TransformFinalBlock(new byte[0], 0, 0); // 完成 MD5 计算的最后一个块

        return Task.FromResult(BytesToHexString(md5.Hash)); // 将计算得到的哈希值转换为十六进制字符串并返回
    }

    /// <summary>
    /// 异步计算 SHA-1 哈希值
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public Task<string> CalculateSHA1HashAsync(string filePath)
    {
        using var sha1 = SHA1.Create();
        using FileStream fs = File.OpenRead(filePath);
        byte[] buffer = new byte[BlockSize];
        int bytesRead = 0;

        while ((bytesRead = fs.Read(buffer, 0, BlockSize)) > 0)
        {
            sha1.TransformBlock(buffer, 0, bytesRead, null, 0);
        }

        sha1.TransformFinalBlock(new byte[0], 0, 0);
        return Task.FromResult(BytesToHexString(sha1.Hash));
    }


    /// <summary>
    /// 计算指定文件夹下所有文件的 MD5 和 SHA-1 哈希值，并将结果存储在一个字典中
    /// </summary>
    /// <param name="folderPath"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, Dictionary<string, string>>> CalculateFolderFileHashesAsync(string folderPath)
    {
        Dictionary<string, Dictionary<string, string>> fileHashes = new Dictionary<string, Dictionary<string, string>>();

        try
        {
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

            await Task.WhenAll(files.Select(async filePath =>
            {
                Dictionary<string, string> hashValues = new Dictionary<string, string>
                {
                    { "md5", await CalculateMD5HashAsync(filePath) },
                    { "sha1", await CalculateSHA1HashAsync(filePath) }
                };

                lock (fileHashes)
                {
                    fileHashes.Add(filePath, hashValues);
                }
            }));

        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogError("文件夹不存在：" + folderPath);
        }
        catch (IOException ex)
        {
            Debug.LogError("计算哈希值时发生IO异常：" + ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("计算哈希值时发生异常：" + ex.Message);
        }

        return fileHashes;
    }


    // 计算文件夹哈希
    public string CalculateFolderHash(Dictionary<string, Dictionary<string, string>> fileHashes)
    {
        List<byte[]> hashBytesList = new List<byte[]>();

        foreach (var kvp in fileHashes)
        {
            string filePath = kvp.Key;
            Dictionary<string, string> hashValues = kvp.Value;

            byte[] hashBytes = Encoding.UTF8.GetBytes(filePath + hashValues["md5"] + hashValues["sha1"]);
            hashBytesList.Add(hashBytes);
        }

        byte[] combinedHashBytes = CombineHashBytes(hashBytesList);

        return BytesToHexString(combinedHashBytes);
    }

    // 将多个字节数组合并为一个字节数组
    private static byte[] CombineHashBytes(List<byte[]> hashBytesList)
    {
        int totalLength = hashBytesList.Sum(bytes => bytes.Length);
        byte[] combinedBytes = new byte[totalLength];
        int currentIndex = 0;

        foreach (byte[] hashBytes in hashBytesList)
        {
            Buffer.BlockCopy(hashBytes, 0, combinedBytes, currentIndex, hashBytes.Length);
            currentIndex += hashBytes.Length;
        }

        return combinedBytes;
    }

    // 将字节数组转换为十六进制字符串
    private static string BytesToHexString(byte[] bytes)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("x2"));
        }
        return sb.ToString();
    }
}
