using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ZFramework
{
	public static class MD5Helper
	{
		public static string FileMD5(string filePath)
		{
			byte[] retVal;
            using (FileStream file = new FileStream(filePath, FileMode.Open))
			{
				using (MD5 md5 = MD5.Create())
				{
					retVal = md5.ComputeHash(file);
				}
            }

            StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in retVal)
			{
                stringBuilder.Append(b.ToString("x2"));
            }
			return stringBuilder.ToString();

		}
	}
}
