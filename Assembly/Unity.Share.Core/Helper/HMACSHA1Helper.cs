using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ZFramework
{
    public class HMACSHA1Helper
    {
        private HMACSHA1 hmacSha1;

        public HMACSHA1Helper(byte[] key)
        {
            hmacSha1 = new HMACSHA1(key);
        }
        public HMACSHA1Helper(string key)
        {
            hmacSha1 = new HMACSHA1(Encoding.UTF8.GetBytes(key));
        }

        public byte[] ComputeHashBytesToBytes(byte[] body)
        {
            return hmacSha1.ComputeHash(body);
        }

        public byte[] ComputeHashStringToBytes(string body)
        {
            return ComputeHashBytesToBytes(Encoding.UTF8.GetBytes(body));
        }

        public string ComputeHashStringToHexString(string body)
        {
            byte[] result = ComputeHashStringToBytes(body);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in result)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        ~HMACSHA1Helper()
        {
            hmacSha1.Clear();
        }

    }
}
