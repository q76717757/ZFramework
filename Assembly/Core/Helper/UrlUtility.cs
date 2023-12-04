using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ZFramework
{
    /// <summary>
    /// 根据编码规则,实际上完整的URL需要分块进行编码
    /// </summary>
    public static class UrlUtility
    {
        ///  URL Encode: 
        ///  未保留字符:[a~z][A~Z][0~9][._-~]
        ///  保留字符:!*'();:@&=+$,/?#[]
        ///  除了Reserved(保留字符)和Unreserved(非保留字符)之外的所有字符，均需要percent编码；
        ///  某些情况下Reserved(保留字符)也需要进行percent编码：
        ///  当Reserved(保留字符)不用于URL分隔符，而是用于其他的位置，不代表某种特性的含义时，需要进行percent编码。
        ///  例如：保留字符用于URL请求query后面的value中时，要对此时用到的Reserved(保留字符)做percent编码；
        ///  
        ///  空字符用+或者%20代替
        ///  rfc3986标准，空格在进行编码时，编码后对应为%20
        ///  根据W3C标准,提交表单时请求时Content-Type：application/x-www-form-urlencoded的情况下，URL请求查询字符串中出现空格时，需替换为+
        ///  
        ///  因此，针对完整URL则要分块进行编码：path块，query块，fragment块，host块
        ///  path块，需要以'/'进行分割，即 '/' 不需要编码，其它均需要编码
        ///  query块，需要以'&'分割一对对key=value；key 和 value 需要编码, '='不编码   &amp; = '&' ，在HTML中的'&用&amp; 来表示
        ///  fragment，全部进行编码
        ///  
        ///  URL Decode:
        ///  不需要判断是否是特殊字符，因为其解码规则很简单，直接根据内容中是否出现%来判断是否需要解码，
        ///  还是直接输出：若出现了%，则连续读出其后两位进行解码；反之，直接输出

        public const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        public const string ReservedChars = "!*'();:@&=+$,/?#[]";
        public const string urlSpace = "%20";

        /// <summary>
        /// 除保留字符和非保留字符外全部百分比编码
        /// </summary>
        public static string Encode_ExcludedAllReserved(string value)
        {
            return Encode_ExcludedCustomReserved(value, ReservedChars, Encoding.UTF8);
        }

        /// <summary>
        /// 除非保留字符[a~z][A~Z][0~9][._-~]外  自行控制保留字符面对各种情况  其余字符全部进行编码
        /// uncodeChars  将不希望进行百分号编码的保留符号加进这个字符串中
        /// </summary>
        public static string Encode_ExcludedCustomReserved(string value,string uncodeChars, Encoding encoding)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            StringBuilder result = new StringBuilder(value.Length * 2);
            byte[] bytes = encoding.GetBytes(value);
            byte space = 32;//空格
            bool hasReservedChars = !string.IsNullOrEmpty(uncodeChars);

            foreach (byte b in bytes)
            {
                char ch = (char)b;
                if (UnreservedChars.IndexOf(ch) != -1 || (hasReservedChars && uncodeChars.IndexOf(ch) != -1))
                {
                    result.Append(ch);
                }
                else
                {
                    if (b == space)
                    {
                        result.Append(urlSpace);
                    }
                    else
                    {
                        result.Append('%').Append(b.ToString("X2", CultureInfo.InvariantCulture));
                    }
                }
            }
            return result.ToString();
        }

        public static string Decode(string valueEncode)
        {
            if (string.IsNullOrEmpty(valueEncode))
            {
                return string.Empty;
            }
            valueEncode.Replace('+', ' ');
            return Uri.UnescapeDataString(valueEncode);
        }
    }

}
