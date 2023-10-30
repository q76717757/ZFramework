using System;
using System.Collections.Generic;
using System.Text;

namespace ZFramework
{
    public sealed class StringUtility
    {
        //按字典序对比字符串
        public static int Compare(string strA, string strB, bool ignoreCase)
        {
            if (strA == null || strB == null)
            {
                throw new Exception("strA = null or strA = null");
            }

            if (ignoreCase)
            {
                strA = strA.ToLower();
                strB = strB.ToLower();
            }

            int strALen = strA.Length;
            int strBLen = strB.Length;

            for (int i = 0, size = strALen > strBLen ? strBLen : strALen; i < size; i++)
            {
                int temp1 = (int)strA[i];
                int temp2 = (int)strB[i];

                if (temp1 > temp2)
                {
                    return 1;
                }
                else if (temp1 < temp2)
                {
                    return -1;
                }
            }
            if (strALen > strBLen)
            {
                return 1;
            }

            if (strALen < strBLen)
            {
                return -1;
            }
            return 0;
        }
    }
}
