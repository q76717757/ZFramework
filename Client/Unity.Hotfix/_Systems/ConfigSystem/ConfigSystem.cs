using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace ZFramework
{
    public static class ConfigSystem
    {
        public static void LoadFromStreamingAssets(this ConfigComponent component)
        {
            var path = Path.Combine(Application.streamingAssetsPath, "Config/Config.ini");

            //临时生成一个用一下
            if (!File.Exists(path))
            {
                var a = new ConfigComponent().ToJson(true);
                File.WriteAllText(path, a, Encoding.UTF8);
            }

            var configText = File.ReadAllText(path, Encoding.UTF8);

            var value = configText.ToObject<ConfigComponent>();
            component.versionFileURL = value.versionFileURL;
            component.md5FileURL = value.md5FileURL;
        }
    }
}
