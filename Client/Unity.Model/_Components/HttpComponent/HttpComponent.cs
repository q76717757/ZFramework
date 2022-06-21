using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public struct HttpCallback
    {
        public bool success;
        public string msg;
        public byte[] data;
    }

    public class HttpComponent : Component
    {
        public const string HTTPURP = "http://120.79.82.49:8880/ChongqingdaxueVirtualExperiment/";
    }
}
