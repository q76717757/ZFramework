using System.Collections;
using System.Collections.Generic;

namespace ZFramework
{
    public interface IUrlHelper
    {
        string Decode(string url);
    }

    public static class UrlUtility
    {
        public static IUrlHelper coder { get; set; }
        public static string Decode(string url)
        {
            return coder.Decode(url);
        }
    }
}
