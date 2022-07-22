using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

namespace ZFramework
{
    public class BundleSystemAwake : AwakeSystem<BundleComponent>
    {
        public override void Awake(BundleComponent component)
        {
            //switch (Define.RunMode)
            //{
            //    case CompileMode.Development:
            //        //var bundlenames = AssetBundleLoader.GetAllAssetBundleNames();//编辑器下不用加载依赖  全部都在工程中
            //        break;
            //    case CompileMode.Release:
            //        {
            //            //var bundlename = AssetBundle.LoadFromFile("Assets/Bundle/*.*");//直接load本地的bundle也可以把资源读出来

            //            //var ab_abm = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "Bundle/_Bundles"));//发布模式需要构建依赖映射
            //            //var abm = ab_abm.LoadAsset<AssetBundleManifest>("assetbundlemanifest");//清单
            //            //var abs = abm.GetAllAssetBundles();
            //            //foreach (var bundle in abs)
            //            //{
            //            //    var ab = AssetBundle.LoadFromFile(bundle);
            //            //    var path = ab.GetAllAssetNames();
            //            //}
            //        }
            //        break;
            //}
        }
    }

    public static class BundleSystem
    {
        public static UnityEngine.Object LoadAsset(this BundleComponent component, string bundleName, string assetName)
        {
            //if (Define.IsEditor)
            //{
            //    return AssetBundleLoader.LoadAsset(bundleName, assetName);
            //}
            //else
            //{
                return Resources.Load("UI/" + assetName);//资源管理策略还没写  为了发包 临时用一下 目前只有UI再用
            //}
        }
    }
}
