using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZFramework;

namespace ZFramework {
    public class AssetModificationProcessorEditor : UnityEditor.AssetModificationProcessor
    {
//#if UNITY_2020_2_OR_NEWER
//        //private static bool IsOpenForEdit(string[] paths, List<string> outNotEditablePaths, StatusQueryOptions statusQueryOptions)
//        //{
//        //    Debug.Log("IsOpenForEdit:");
//        //    foreach (var path in paths)
//        //        Debug.Log(path);
//        //    return true;
//        //}
//#else
//        ////当 Unity 检查资源以确定是否应禁用编辑器时，则会调用此方法
//        //private static bool IsOpenForEdit(string a,string b) {
//        //    ZLog.Info(a);
//        //    ZLog.Info(b);
//        //    return true;
//        //}
//#endif

        ////Unity 在即将创建未导入的资源（例如，.meta 文件）时调用此方法
        //private static void OnWillCreateAsset(string path)
        //{
        //    path =  path.Replace(".meta", "");
        //    var ext = Path.GetExtension(path);
        //    if (ext == ".cs")//创建脚本
        //    {
        //        //RegeditIO.ImportScript(path);
        //    }
        //}

        ////当 Unity 即将从磁盘中删除资源时，则会调用此方法
        //private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions option)
        //{
        //    Debug.Log("资源即将被删除 : " + path);
        //    return AssetDeleteResult.DidNotDelete;
        //}

        ////Unity 即将在磁盘上移动资源时会调用此方法
        //private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        //{
        //    Debug.Log("Source path: " + sourcePath + ". Destination path: " + destinationPath + ".");
        //    AssetMoveResult assetMoveResult = AssetMoveResult.DidNotMove;
        //    return assetMoveResult;
        //}

        //////Unity 即将向磁盘写入序列化资源或场景文件时会调用此方法
        //static string[] OnWillSaveAssets(string[] paths)
        //{
        //    Debug.Log("OnWillSaveAssets");
        //    foreach (string path in paths)
        //        Debug.Log(path);
        //    return paths;
        //}
    }

}