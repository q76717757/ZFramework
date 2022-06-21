#if UNITY_2020_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace ZFramework
{
    [UnityEditor.AssetImporters.ScriptedImporter(1, "ini")]
    public class IniImporter: UnityEditor.AssetImporters.ScriptedImporter
    {
        //拓展INI文件 让unity把INI识别为TextAsset
        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            TextAsset subAsset = new TextAsset(File.ReadAllText(ctx.assetPath, Encoding.UTF8));
            ctx.AddObjectToAsset("text", subAsset);
            ctx.SetMainObject(subAsset);
        }
    }
}
#endif
