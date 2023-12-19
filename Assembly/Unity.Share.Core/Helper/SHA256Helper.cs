using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ZFramework
{
    public static class SHA256Helper
    {
        /// <summary>
        /// SHA256 不可逆加密
        /// </summary>
        public static string Encrypt(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] bytes256 = SHA256Managed.Create().ComputeHash(bytes);
            return Convert.ToBase64String(bytes256);
        }
    }
}
