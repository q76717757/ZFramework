using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public class UrlHelper : IUrlHelper
    {
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Reg()
        {
            UrlUtility.coder = new UrlHelper();
        }

        public string Decode(string url)
        {
            return UnityEngine.Networking.UnityWebRequest.EscapeURL(url);
        }
    }
}
