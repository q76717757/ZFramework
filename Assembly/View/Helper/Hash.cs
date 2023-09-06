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
    private const int BlockSize = 32768; // ÿ�����ݿ�Ĵ�С 32kb

    private async void Start()
    {
        string folderPath = "path/to/large/file";  // �޸ĳ��Լ����ļ�·��

        Dictionary<string, Dictionary<string, string>> fileHashes = await CalculateFolderFileHashesAsync(folderPath);

        foreach (var kvp in fileHashes)
        {
            string filePath = kvp.Key;
            Dictionary<string, string> hashValues = kvp.Value;

            Debug.Log("�ļ�·��: " + filePath);
            Debug.Log("MD5 ��ϣֵ: " + hashValues["md5"]);
            Debug.Log("SHA-1 ��ϣֵ: " + hashValues["sha1"]);
        }

        string folderHash = CalculateFolderHash(fileHashes);

        Debug.Log("�ļ��й�ϣֵ: " + folderHash);
    }


    //// ���� MD5 ��ϣֵ
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

    //// ���� SHA-1 ��ϣֵ
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
    /// �첽���� MD5 ��ϣֵ
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public Task<string> CalculateMD5HashAsync(string filePath)
    {
        using var md5 = MD5.Create(); // ���� MD5 ʵ��
        using FileStream fs = File.OpenRead(filePath); // ���ļ�����ȡ�ļ�����
        byte[] buffer = new byte[BlockSize]; // �������������ڴ洢��ȡ�����ݿ�
        int bytesRead = 0;

        while ((bytesRead = fs.Read(buffer, 0, BlockSize)) > 0) // �Կ�Ϊ��λ��ȡ�ļ�����
        {
            md5.TransformBlock(buffer, 0, bytesRead, null, 0); // ���� MD5 ������м���
        }

        md5.TransformFinalBlock(new byte[0], 0, 0); // ��� MD5 ��������һ����

        return Task.FromResult(BytesToHexString(md5.Hash)); // ������õ��Ĺ�ϣֵת��Ϊʮ�������ַ���������
    }

    /// <summary>
    /// �첽���� SHA-1 ��ϣֵ
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
    /// ����ָ���ļ����������ļ��� MD5 �� SHA-1 ��ϣֵ����������洢��һ���ֵ���
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
            Debug.LogError("�ļ��в����ڣ�" + folderPath);
        }
        catch (IOException ex)
        {
            Debug.LogError("�����ϣֵʱ����IO�쳣��" + ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("�����ϣֵʱ�����쳣��" + ex.Message);
        }

        return fileHashes;
    }


    // �����ļ��й�ϣ
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

    // ������ֽ�����ϲ�Ϊһ���ֽ�����
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

    // ���ֽ�����ת��Ϊʮ�������ַ���
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
