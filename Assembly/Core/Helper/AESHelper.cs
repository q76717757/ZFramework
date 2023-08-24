using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ZFramework
{
    /// <summary>
    /// AES加解密算法
    /// </summary>
    public class AESHelper
    {
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <param name="key">密钥，长度16byte</param>
        /// <param name="IV">初始化向量，长度16byte</param>
        /// <returns>返回密文</returns>
        public static string Encrypt(string plaintext, string key, string iv)
        {
            if (string.IsNullOrEmpty(plaintext)) return string.Empty;
            try
            {
                byte[] btKey = Encoding.UTF8.GetBytes(key);
                byte[] btIV = Encoding.UTF8.GetBytes(iv);

                byte[] inputByteArray = Encoding.UTF8.GetBytes(plaintext);
                using (AesCryptoServiceProvider provider = new AesCryptoServiceProvider())
                {
                    using (MemoryStream mStream = new MemoryStream())
                    {
                        CryptoStream cStream = new CryptoStream(mStream, provider.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write);
                        cStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cStream.FlushFinalBlock();
                        cStream.Close();
                        return Convert.ToBase64String(mStream.ToArray());
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        // <summary>
        /// AES解密
        /// </summary>
        /// <param name="ciphertext">密文</param>
        /// <param name="key">密钥，长度16byte</param>
        /// <param name="iv">初始化向量，长度16byte</param>
        /// <returns>返回明文</returns>
        public static string Decrypt(string ciphertext, string key, string iv)
        {
            if (string.IsNullOrEmpty(ciphertext)) return string.Empty;
            try
            {
                byte[] btKey = Encoding.UTF8.GetBytes(key);
                byte[] btIV = Encoding.UTF8.GetBytes(iv);

                byte[] inputByteArray = Convert.FromBase64String(ciphertext);
                using (AesCryptoServiceProvider provider = new AesCryptoServiceProvider())
                {
                    using (MemoryStream mStream = new MemoryStream())
                    {
                        CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write);
                        cStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cStream.FlushFinalBlock();
                        cStream.Close();
                        return Encoding.UTF8.GetString(mStream.ToArray());
                    }
                }
            }
            catch
            {
            }
            return string.Empty;
        }
    }
}
