using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace ZFramework
{
    public class AssetBundleLoader
    {
        public static string[] GetAllAssetBundleNames()
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAllAssetBundleNames();
#endif
            return new string[0];
        }

        public static string[] GetAssetPathsFromAssetBundle(string assetBundleName)
        {
#if UNITY_EDITOR
            assetBundleName = assetBundleName.ToLower();
            return UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
#endif
            return new string[0];
        }

        public static string[] GetAssetPathsFromAssetBundleAndAssetName(string assetBundleName, string assetName)
        {
#if UNITY_EDITOR
            assetBundleName = assetBundleName.ToLower();
            return UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
#endif
            return new string[0];
        }

        public static string[] GetAssetBundleDependencies(string assetBundleName,bool recursive)
        {
#if UNITY_EDITOR
            assetBundleName = assetBundleName.ToLower();
            return UnityEditor.AssetDatabase.GetAssetBundleDependencies(assetBundleName, recursive);
#endif
            return new string[0];
        }

        public static UnityEngine.Object LoadAsset(string assetBundleName, string assetName)
        {
#if UNITY_EDITOR
            var paths = GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
            if (paths.Length > 0)
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(paths[0]);
            }
#endif
            return null;
        }

    }
}
