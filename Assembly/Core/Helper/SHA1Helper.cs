using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ZFramework
{
    public class SHA1Helper
    {
        SHA1 sha1;

        public SHA1Helper()
        {
            sha1 = SHA1.Create();
        }

        public byte[] ComputeHashBytesToBytes(byte[] body)
        {
            return sha1.ComputeHash(body);
        }

        public byte[] ComputeHashStringToBytes(string body)
        {
            return ComputeHashBytesToBytes(Encoding.UTF8.GetBytes(body));
        }

        public string ComputeHashBytesToHexString(byte[] body)
        {
            var bytes = ComputeHashBytesToBytes(body);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
        public string ComputeHashStringToHexString(string body)
        {
            return ComputeHashBytesToHexString(Encoding.UTF8.GetBytes(body));
        }

        ~SHA1Helper()
        {
            sha1.Dispose();
        }
    }
}
