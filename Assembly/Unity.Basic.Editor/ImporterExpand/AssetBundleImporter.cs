using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ZFramework.Editor
{
    [ScriptedImporter(1, "ab")]
    public class AssetBundleImporter : ScriptedImporter
    {
        //拓展ab文件  让unity把ab后缀的识别为ab包
        public override void OnImportAsset(AssetImportContext ctx)
        {
            byte[] bytes = File.ReadAllBytes(ctx.assetPath);

            var ab =  AssetBundle.LoadFromFile(ctx.assetPath);
            var s =  ab.GetAllAssetNames();

            ab.Unload(true);

            StringBuilder sb = new StringBuilder();
            foreach (var item in s)
            {
                sb.AppendLine(item);
            }

            TextAsset subAsset = new TextAsset(sb.ToString());
            ctx.AddObjectToAsset("text", subAsset);
            ctx.SetMainObject(subAsset);


        }
    }
}
