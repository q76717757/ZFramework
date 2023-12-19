using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ZFramework
{
    /// <summary>
    /// RAS 非对称加密
    /// </summary>
    public class RSAHelper
    {
        //private void Example()
        //{
        //    KeyValuePair<string, string> rsaKey = GenerateRSAKey();
        //    string encrypt = RSAEncrypt("你好", rsaKey.Key);
        //    Debug.Log(encrypt);
        //    Debug.Log(RSADecrypt(encrypt, rsaKey.Value));
        //}

        /// <summary>
        /// 生成RSA key
        /// KeyValuePair <publicKey,privateKey>
        /// </summary>
        /// <returns></returns>
        private static KeyValuePair<string, string> GenerateRSAKey()
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            //false表示仅包含公钥        
            string publicKey = RSA.ToXmlString(false);
            //true表示同时包含公钥和私钥
            string privateKey = RSA.ToXmlString(true);
            return new KeyValuePair<string, string>(publicKey, privateKey);
        }
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <param name="publicKey">公开的秘钥</param>
        /// <returns></returns>
        public static string RSAEncrypt(string plaintext, string publicKey)
        {
            if (string.IsNullOrEmpty(plaintext)) return string.Empty;
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(publicKey);
                byte[] dataToEncrypt = System.Text.Encoding.UTF8.GetBytes(plaintext);
                byte[] resultBytes = rsa.Encrypt(dataToEncrypt, false);
                return Convert.ToBase64String(resultBytes);
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="ciphertext">密文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string RSADecrypt(string ciphertext, string privateKey)
        {
            if (string.IsNullOrEmpty(ciphertext)) return string.Empty;
            try
            {
                byte[] dataToDecrypt = Convert.FromBase64String(ciphertext);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(privateKey);
                byte[] resultData = rsa.Decrypt(dataToDecrypt, false);
                return System.Text.Encoding.UTF8.GetString(resultData);
            }
            catch { }
            return string.Empty;
        }

    }
}