using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public class ConfigComponent : ComponentData
    {
        public string versionFileURL = "http://192.168.0.189/Downloads/version.ini";
        public string md5FileURL = "http://192.168.0.189/Downloads/md5.ini";

        //public Dictionary<string, byte[]> configs;//所有的配置表
    }
}
